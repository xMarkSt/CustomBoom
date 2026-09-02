using System.IO.Compression;
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
    /// Get the ghost replay binary for a specific opponent within a tournament.
    /// </summary>
    /// <param name="dto">Tournament uuid and opponent uuid.</param>
    /// <returns>
    /// The gzip-decompressed ghost binary (the client stores it compressed and expects the
    /// decompressed replay back, matching the original PHP Ghost::getDataAttribute accessor),
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

        var data = standing?.Ghost?.Data;
        return data == null ? null : GzipDecompress(data);
    }

    /// <summary>
    /// Gzip-decompress the stored ghost blob. If the data is not gzip-framed it is returned
    /// unchanged, so legacy/uncompressed rows do not crash the download.
    /// </summary>
    private static byte[] GzipDecompress(byte[] data)
    {
        // gzip magic bytes: 0x1f 0x8b. Anything else is not a gzip stream.
        if (data.Length < 2 || data[0] != 0x1f || data[1] != 0x8b)
            return data;

        using var source = new MemoryStream(data);
        using var gzip = new GZipStream(source, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>
    /// Player updates their score/ghost mid-tournament. The time, styles, and ghost all belong to
    /// a single run (the styles are encoded in the ghost header), so they are overwritten together
    /// only when the new time is strictly better; equal or slower times leave the standing
    /// untouched. Unlike Join, it never creates a standing — the player must already be in the
    /// tournament.
    /// </summary>
    /// <returns>The tournament result with standings and player rank. Null if tournament or standing not found.</returns>
    public async Task<JoinTournamentResponseDto?> Update(UpdateTournamentDto dto, Player player)
    {
        var tournament = await _repository.GetAll<Tournament>()
            .Include(t => t.Standings)
            .ThenInclude(s => s.Player)
            .FirstOrDefaultAsync(t => t.Uuid == dto.TournamentUuid);

        if (tournament == null)
            return null;

        // Find the player's existing standing; update never creates one.
        var standing = tournament.Standings.FirstOrDefault(s => s.UserId == player.Id);
        if (standing == null)
            return null;

        // Only overwrite when the new time is strictly better; equal or slower times leave the
        // run (time + styles + ghost) untouched so the ghost never diverges from its recorded time.
        if (dto.Time >= standing.Time)
            return BuildJoinResponse(tournament, player);

        // Replace ghost (delete old one).
        var oldGhost = _repository.GetById<Ghost>(standing.GhostId);
        if (oldGhost != null)
            _repository.Remove(oldGhost);

        // Overwrite the whole run together (styles are part of the ghost header).
        var ghostData = await dto.GhostData.GetBytes();
        standing.Ghost = new Ghost { Data = ghostData };
        standing.Time = dto.Time;
        standing.HeroStyle = dto.HeroStyle;
        standing.WheelStyle = dto.WheelStyle;
        standing.EngineStyle = dto.EngineStyle;

        _repository.Update(standing);
        await _repository.SaveAsync();

        // Player now holds rank #1?
        var fastest = tournament.Standings.OrderBy(s => s.Time).FirstOrDefault();
        if (fastest != null && fastest.UserId == player.Id)
        {
            // TODO: discord broadcast (rank #1)
        }

        return BuildJoinResponse(tournament, player);
    }

    /// <summary>
    /// Post-tournament results: top 3 standings plus the player's own standing, per requested
    /// tournament, keyed by the tournament's uuid. Also recomputes the player's aggregate
    /// tournament stats (wins, tournaments played) across all tournaments they've competed in.
    /// </summary>
    public async Task<TournamentResultsDto> Results(GetTournamentResultsDto dto, Player player)
    {
        var tournaments = await _repository.GetAll<Tournament>()
            .Include(t => t.Standings)
            .ThenInclude(s => s.Player)
            .Include(t => t.TournamentGroup)
            .Where(t => dto.TournamentUuids.Contains(t.Uuid))
            .ToListAsync();

        var response = new TournamentResultsDto();
        foreach (var tournament in tournaments)
        {
            response[tournament.Uuid] = BuildResultsEntry(tournament, player);
        }

        await UpdatePlayerTournamentStats(player);

        return response;
    }

    private TournamentResultDto BuildResultsEntry(Tournament tournament, Player player)
    {
        var sorted = RankStandings(tournament.Standings).ToList();
        var selfStanding = sorted.FirstOrDefault(s => s.UserId == player.Id);
        var rank = selfStanding != null ? sorted.IndexOf(selfStanding) + 1 : 0;

        var podium = sorted.Take(3).ToList();
        var standings = podium.Select((s, index) =>
        {
            var standingDto = _mapper.Map<StandingDto>(s);
            standingDto.Rank = index + 1;
            standingDto.IsSelf = s.UserId == player.Id;
            return standingDto;
        }).ToList();

        // Player's own standing isn't in the podium: append it with their real (possibly >3) rank.
        if (selfStanding != null && !podium.Contains(selfStanding))
        {
            var standingDto = _mapper.Map<StandingDto>(selfStanding);
            standingDto.Rank = rank;
            standingDto.IsSelf = true;
            standings.Add(standingDto);
        }

        return new TournamentResultDto
        {
            Completed = tournament.TournamentGroup.EndsAt <= DateTime.UtcNow ? 1 : 0,
            Rank = rank,
            Standings = standings
        };
    }

    /// <summary>
    /// Order standings within a single tournament by rank: fastest time first, ties broken by
    /// whoever submitted first (lower id). Exact time ties aren't rare on some levels, so a
    /// plain "time equals the best time" check would credit every tied player as rank 1 —
    /// this mirrors PHP's Standing::rank accessor (ordered by time, ties broken by row order),
    /// which only ever awards rank 1 to a single standing.
    /// </summary>
    private static IEnumerable<Standing> RankStandings(IEnumerable<Standing> standings) =>
        standings.OrderBy(s => s.Time).ThenBy(s => s.Id);

    /// <summary>
    /// Recompute the player's tournament stats across every tournament they've ever stood in:
    /// how many they've played, and how many they won (their standing is rank 1 in that
    /// tournament).
    /// </summary>
    private async Task UpdatePlayerTournamentStats(Player player)
    {
        var playerStandings = await _repository.GetAll<Standing>()
            .Where(s => s.UserId == player.Id)
            .ToListAsync();

        var tournamentIds = playerStandings.Select(s => s.TournamentId).Distinct().ToList();
        var winningStandingIds = (await _repository.GetAll<Standing>()
                .Where(s => tournamentIds.Contains(s.TournamentId))
                .ToListAsync())
            .GroupBy(s => s.TournamentId)
            .Select(g => RankStandings(g).First().Id)
            .ToHashSet();

        player.WcPlayed = playerStandings.Count;
        player.WcWon = playerStandings.Count(s => winningStandingIds.Contains(s.Id));

        _repository.Update(player);
        await _repository.SaveAsync();
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
        var sortedDtos = RankStandings(tournament.Standings)
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