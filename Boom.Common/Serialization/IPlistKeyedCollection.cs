namespace Boom.Common.Serialization;

/// <summary>
/// A plist payload whose root has no fixed set of properties: it's a dictionary keyed by a
/// runtime value (e.g. a tournament uuid) rather than named fields. PlistSerializationService
/// special-cases this instead of reflecting over properties.
/// </summary>
public interface IPlistKeyedCollection : IPlistSerializable
{
    IEnumerable<KeyValuePair<string, IPlistSerializable>> Entries { get; }
}
