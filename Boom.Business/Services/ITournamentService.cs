using Boom.Common;
using Boom.Infrastructure.Data.Entities;
using Claunia.PropertyList;

namespace Boom.Business.Services;

public interface ITournamentService
{
    Task<TournamentGroup?> GetScheduled();

    /// <summary>
    /// Serialize a dto type class to a NSDictionary
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    NSDictionary SerializeToNSDictionary(IPlistSerializable dto);
}