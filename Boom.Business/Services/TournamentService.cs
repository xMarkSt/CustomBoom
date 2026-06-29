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
        var tournament = tournamentGroup.Tournaments.Count != 0
            ? tournamentGroup.Tournaments.First()
            : await CreateTournament(tournamentGroup.Id);

        // Check if player already has a standing
        var standing = tournament.Standings.FirstOrDefault(s => s.UserId == player.Id);
        if (standing != null)
        {
            if (standing.Time <= dto.Time)
                return BuildJoinResponse(tournament, player); // existing time is faster or equal, no update needed
        }
        else
        {
            standing = new Standing();
        }

        // Replace ghost (delete old one if updating)
        if (standing.Id != 0)
        {
            var oldGhost = _repository.GetById<Ghost>(standing.GhostId);
            if (oldGhost != null)
                _repository.Remove(oldGhost);
        }

        var ghostData = await dto.GhostData.GetBytes();
        ApplyJoinToStanding(standing, dto, tournament.Id, player.Id, ghostData);

        // No existing standing, create new
        if (standing.Id == 0)
        {
            _repository.Add(standing);
            standing.Player = player;
        }
        // Update existing standing
        else
        {
            _repository.Update(standing);
        }

        await _repository.SaveAsync();

        // TODO: discord broadcast
        return BuildJoinResponse(tournament, player);
    }
    
    public async Task<JoinTournamentResponseDto?> Reload(ReloadTournamentDto dto, Player player)
    {
        var tournament = await _repository.GetAll<Tournament>()
            .Include(t => t.Standings)
            .ThenInclude(s => s.Player)
            .FirstOrDefaultAsync(t => t.Uuid == dto.TournamentUuid);

        if (tournament == null)
            return null;

        return BuildJoinResponse(tournament, player);
    }

    /// <summary>
    /// Get the raw ghost replay binary for a specific opponent within a tournament.
    /// </summary>
    /// <param name="dto">Tournament uuid and opponent uuid.</param>
    /// <returns>
    /// The stored ghost bytes (gzip-compressed, served as-is to match the original PHP behaviour),
    /// or null if the tournament, the opponent's standing, or its ghost is not found.
    /// </returns>
    public async Task<byte[]?> GetGhost(GhostTournamentDto dto)
    {
        var standing = await _repository.GetAll<Standing>()
            .Include(s => s.Ghost)
            .Include(s => s.Tournament)
            .Include(s => s.Player)
            .FirstOrDefaultAsync(s =>
                s.Tournament.Uuid == dto.TournamentUuid &&
                s.Player.Uuid == dto.OpponentUuid);

        return standing?.Ghost?.Data;
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
        _repository.Add(tournamentGroup);
        await _repository.SaveAsync();

        return tournamentGroup;
    }

    private static void ApplyJoinToStanding(Standing standing, JoinTournamentDto dto, long tournamentId, long userId, byte[] ghostData)
    {
        standing.Ghost = new Ghost { Data = ghostData };
        standing.TournamentId = tournamentId;
        standing.UserId = userId;
        standing.Time = dto.Time;
        standing.HeroStyle = dto.HeroStyle;
        standing.WheelStyle = dto.WheelStyle;
        standing.EngineStyle = dto.EngineStyle;
    }

    private JoinTournamentResponseDto BuildJoinResponse(Tournament tournament, Player player)
    {
        var sortedDtos = tournament.Standings
            .OrderBy(s => s.Time)
            .Select((s, index) =>
            {
                var dto = _mapper.Map<StandingDto>(s);
                dto.Rank = index + 1;
                dto.IsSelf = s.UserId == player.Id;
                return dto;
            })
            .ToList();

        var standings = sortedDtos.ToList();
        if (sortedDtos.Count <= 0)
            return new JoinTournamentResponseDto
            {
                Tournament = _mapper.Map<TournamentDto>(tournament),
                Standings = standings,
                Rank = sortedDtos.FirstOrDefault(s => s.IsSelf)?.Rank ?? 0
            };
        // Prepend a copy of the #1 standing at rank 0
        var first = sortedDtos[0];
        standings.Insert(0, new StandingDto
        {
            Id = first.Id,
            TournamentId = first.TournamentId,
            UserId = first.UserId,
            GhostId = first.GhostId,
            Time = first.Time,
            HeroStyle = first.HeroStyle,
            WheelStyle = first.WheelStyle,
            EngineStyle = first.EngineStyle,
            CreatedAt = first.CreatedAt,
            UpdatedAt = first.UpdatedAt,
            BoomUser = first.BoomUser,
            IsSelf = first.IsSelf,
            Rank = 0
        });

        return new JoinTournamentResponseDto
        {
            Tournament = _mapper.Map<TournamentDto>(tournament),
            Standings = standings,
            Rank = sortedDtos.FirstOrDefault(s => s.IsSelf)?.Rank ?? 0
        };
    }

    private async Task<Tournament> CreateTournament(long groupId, int eloGroup = 1, int cheaters = 0)
    {
        var tournament = new Tournament
        {
            TournamentGroupId = groupId,
            Uuid = Guid.NewGuid(),
            EloGroup = eloGroup,
            Cheaters = cheaters
        };

        _repository.Add(tournament);
        await _repository.SaveAsync();

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