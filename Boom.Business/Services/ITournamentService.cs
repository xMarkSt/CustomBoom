using Boom.Common.DTOs.Response;

namespace Boom.Business.Services;

public interface ITournamentService
{
    Task<ScheduleDto> GetSchedule();
}