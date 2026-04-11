using AutoMapper;
using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;
using Boom.Common.Extensions;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using JoinTournamentDto = Boom.Common.DTOs.Request.JoinTournamentDto;
using JoinTournamentResponseDto = Boom.Common.DTOs.Response.JoinTournamentDto;

namespace Boom.Business.Services;

public class TournamentService : ITournamentService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    public TournamentService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Get the currently scheduled tournament group
    /// </summary>
    /// <returns></returns>
    public async Task<ScheduleDto> GetSchedule()
    {
        var current = await _repository.GetAll<TournamentGroup>()
            .Where(x => x.EndsAt > DateTime.UtcNow)
            .Include(x => x.LevelTarget.Level)
            .Include(x => x.LevelTarget.Level.Theme)
            .Include(x => x.LevelTarget.Level.Background)
            .Include(x => x.LevelTarget.Target)
            .OrderBy(x => x.EndsAt)
            .FirstOrDefaultAsync();

        return new ScheduleDto
        {
            Schedule = current != null ? [_mapper.Map<TournamentGroupDto>(current)] : []
        };
    }

    /// <summary>
    /// Player joins a tournament
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="player"></param>
    /// <returns>The tournament result with standings and player rank. Null if group not found or ended.</returns>
    public async Task<JoinTournamentResponseDto?> Join(JoinTournamentDto dto, Player player)
    {
        // Get the tournament group by uuid
        var tournamentGroup = await _repository.GetAll<TournamentGroup>()
            .Include(tg => tg.Tournaments)
            .ThenInclude(t => t.Standings)
            .ThenInclude(s => s.Player)
            .FirstOrDefaultAsync(tg => tg.Uuid == dto.GroupUuid);

        // TournamentGroup not found or already ended
        if (tournamentGroup == null || GroupHasEnded(tournamentGroup))
        {
            return null;
        }

        // Pick or create tournament
        var tournament = tournamentGroup.Tournaments.Any()
            ? tournamentGroup.Tournaments.First()
            : await CreateTournament(tournamentGroup.Id);

        // Check if player already has a standing
        var standing = tournament.Standings.FirstOrDefault(s => s.UserId == player.Id);
        bool isNewStanding;
        if (standing != null)
        {
            if (standing.Time <= dto.Time)
                return BuildJoinResponse(tournament, player); // existing time is faster or equal, no update needed
            isNewStanding = false;
        }
        else
        {
            standing = new Standing();
            isNewStanding = true;
        }

        // Save ghost
        var ghost = new Ghost { Data = await dto.GhostData.GetBytes() };
        await _repository.CreateAsync(ghost);

        // Update standing
        standing.TournamentId = tournament.Id;
        standing.UserId = player.Id;
        standing.GhostId = ghost.Id;
        standing.Time = dto.Time;
        standing.HeroStyle = dto.HeroStyle;
        standing.WheelStyle = dto.WheelStyle;
        standing.EngineStyle = dto.EngineStyle;

        await _repository.CreateAsync(standing);

        if (isNewStanding)
        {
            standing.Player = player;
            tournament.Standings.Add(standing);
        }

        // TODO: discord broadcast
        return BuildJoinResponse(tournament, player);
    }

    private JoinTournamentResponseDto BuildJoinResponse(Tournament tournament, Player player)
    {
        var standingDtos = tournament.Standings
            .OrderBy(s => s.Time)
            .Select((s, index) =>
            {
                var dto = _mapper.Map<StandingDto>(s);
                dto.Rank = index + 1;
                dto.IsSelf = s.UserId == player.Id;
                return dto;
            })
            .ToList();

        return new JoinTournamentResponseDto
        {
            Tournament = _mapper.Map<TournamentDto>(tournament),
            Standings = standingDtos,
            Rank = standingDtos.FirstOrDefault(s => s.IsSelf)?.Rank ?? 0
        };
    }

    public async Task<TournamentGroup> CreateGroup(TimeSpan duration, LevelTarget? levelTarget = null)
    {
        // Pick a random level target if none provided
        if (levelTarget == null)
        {
            levelTarget = await _repository.GetAll<LevelTarget>()
                .OrderBy(x => EF.Functions.Random())
                .FirstOrDefaultAsync();

            if (levelTarget == null)
                throw new InvalidOperationException("No LevelTarget available to assign.");
        }

        var tournamentGroup = new TournamentGroup
        {
            Uuid = Guid.NewGuid(),
            LevelTargetId = levelTarget.Id,
            NoSuper = new Random().NextDouble() < 0.25 // 25% chance
        };

        // Find last tournament to chain times
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var now = DateTime.UtcNow;

        var lastTournament = await _repository.GetAll<TournamentGroup>()
            .Where(tg =>
                    (tg.EndsAt >= today && tg.EndsAt < tomorrow) // ends today
                    || tg.EndsAt > now // not ended yet
            )
            .OrderByDescending(tg => tg.EndsAt)
            .FirstOrDefaultAsync();

        tournamentGroup.StartsAt =
            lastTournament != null ? RoundHour(lastTournament.EndsAt) : RoundHour(DateTime.UtcNow);
        tournamentGroup.EndsAt = tournamentGroup.StartsAt.Add(duration);
        await _repository.CreateAsync(tournamentGroup);

        return tournamentGroup;
    }

    public async Task<Tournament> CreateTournament(long groupId, int eloGroup = 1, int cheaters = 0)
    {
        var tournament = new Tournament
        {
            TournamentGroupId = groupId,
            Uuid = Guid.NewGuid(),
            EloGroup = eloGroup,
            Cheaters = cheaters
        };

        await _repository.CreateAsync(tournament);

        return tournament;
    }

    /// <summary>
    /// Round a DateTime down.
    /// </summary>
    private DateTime RoundHour(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, DateTimeKind.Utc);
    }

    private bool GroupHasEnded(TournamentGroup tournamentGroup)
    {
        return tournamentGroup.EndsAt <= DateTime.UtcNow;
    }
}