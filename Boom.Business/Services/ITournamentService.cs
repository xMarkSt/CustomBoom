using Boom.Common;
using Boom.Common.DTOs;
using Boom.Infrastructure.Data.Entities;
using Claunia.PropertyList;

namespace Boom.Business.Services;

public interface ITournamentService
{
    Task<ScheduleDto> GetScheduled();
}