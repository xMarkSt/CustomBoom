namespace Boom.Common.DTOs.Response;

public class TournamentDto : IPlistSerializable
{
    public int Id { get; set; }
    public int TournamentGroupId { get; set; }
    public Guid Uuid { get; set; }
    public int Users { get; set; }
    public int EloGroup { get; set; }
    public int Cheaters { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}