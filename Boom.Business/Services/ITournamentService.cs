using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;
using JoinTournamentDto = Boom.Common.DTOs.Request.JoinTournamentDto;

namespace Boom.Business.Services;

public interface ITournamentService
{
    Task<ScheduleDto> GetSchedule();
    Task<Boom.Common.DTOs.Response.JoinTournamentDto?> Join(JoinTournamentDto dto, Player player);
    Task<Boom.Common.DTOs.Response.JoinTournamentDto?> Reload(ReloadTournamentDto dto, Player player);
    Task<byte[]?> GetGhost(GhostTournamentDto dto);
    Task<TournamentGroup> CreateGroup(TimeSpan duration, LevelTarget? levelTarget = null);
}