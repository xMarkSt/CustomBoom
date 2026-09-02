using Boom.Common.Serialization;

namespace Boom.Common.DTOs.Response;

/// <summary>
/// The plist root for /tournaments/results has no fixed set of properties: it is a dictionary
/// keyed by tournament uuid, each value the result for that tournament.
/// </summary>
public class TournamentResultsDto : IPlistKeyedCollection
{
    private readonly Dictionary<Guid, TournamentResultDto> _results = new();

    public TournamentResultDto this[Guid tournamentUuid]
    {
        get => _results[tournamentUuid];
        set => _results[tournamentUuid] = value;
    }

    public IEnumerable<Guid> Keys => _results.Keys;

    public IEnumerable<KeyValuePair<string, IPlistSerializable>> Entries =>
        _results.Select(kv => new KeyValuePair<string, IPlistSerializable>(kv.Key.ToString(), kv.Value));
}
