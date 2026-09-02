using Boom.Common.Serialization;

namespace Boom.Common.DTOs.Response;

public class ArticleDto : IPlistSerializable
{
    public long Id { get; set; }
    public long Timestamp { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? Link { get; set; }
    public string? LinkTitle { get; set; }
    public int Popup { get; set; }
}
