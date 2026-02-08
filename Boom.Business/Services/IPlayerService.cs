using Boom.Common.DTOs.Request;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.Services;

public interface IPlayerService
{
    Task<Player> UpdatePlayer(IPlayerInfo playerInfo);
}
