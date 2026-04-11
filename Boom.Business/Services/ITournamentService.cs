using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;
using JoinTournamentDto = Boom.Common.DTOs.Request.JoinTournamentDto;

namespace Boom.Business.Services;

public interface ITournamentService
{
    Task<ScheduleDto> GetSchedule();
    Task<Boom.Common.DTOs.Response.JoinTournamentDto?> Join(JoinTournamentDto dto, Player player);
    Task<TournamentGroup> CreateGroup(TimeSpan duration, LevelTarget? levelTarget = null);
}