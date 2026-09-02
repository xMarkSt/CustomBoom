using Boom.Common.Serialization;

namespace Boom.Common.DTOs.Response;

public class NewsFeedDto : IPlistSerializable
{
    /// <summary>
    /// Either an empty list (serializes to a CFArray) when there are no articles, or a
    /// Dictionary&lt;string, ArticleDto&gt; keyed by "{timestamp}.{id}" (serializes to a CFDictionary) when there are.
    /// </summary>
    public object Articles { get; set; } = new List<ArticleDto>();

    public int Count { get; set; }
    public int New { get; set; }
}
