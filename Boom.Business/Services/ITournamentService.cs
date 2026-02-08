using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;

namespace Boom.Business.Services;

public interface ITournamentService
{
    Task<ScheduleDto> GetSchedule();
    Task<bool> Join(JoinTournamentDto dto);
}