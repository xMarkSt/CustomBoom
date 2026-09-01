namespace Boom.Common.DTOs.Response;

/// <summary>
/// The plist root for /tournaments/results has no fixed set of properties: it is a dictionary
/// keyed by tournament uuid, each value the results for that tournament. Deriving from
/// Dictionary lets PlistSerializationService special-case it instead of reflecting over
/// (nonexistent) named properties.
/// </summary>
public class ResultsResponseDto : Dictionary<Guid, TournamentResultsDto>, IPlistSerializable
{
}
