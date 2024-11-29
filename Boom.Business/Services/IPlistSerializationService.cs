using Boom.Common;
using Claunia.PropertyList;

namespace Boom.Business.Services;

public interface IPlistSerializationService
{
    /// <summary>
    /// Serialize a dto type class to a NSDictionary
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    NSDictionary SerializeToNSDictionary(IPlistSerializable dto);

    string ToPlistString(IPlistSerializable dto);
}