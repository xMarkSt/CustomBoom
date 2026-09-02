using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.Services;

public interface ITournamentService
{
    Task<ScheduleDto> GetSchedule();
    Task<TournamentStandingsDto?> Join(JoinTournamentDto dto, Player player);
    Task<TournamentStandingsDto?> Reload(ReloadTournamentDto dto, Player player);
    Task<TournamentStandingsDto?> Update(UpdateTournamentDto dto, Player player);
    Task<byte[]?> GetGhost(GetTournamentGhostDto dto);
    Task<TournamentResultsDto> Results(GetTournamentResultsDto dto, Player player);
    Task<TournamentGroup> CreateGroup(TimeSpan duration, LevelTarget? levelTarget = null);
}
