using Boom.Common.DTOs.Request;

namespace Boom.Business.Services;

public interface IPlayerService
{
    Task UpdatePlayer(GetScheduleDto dto);
}