namespace Boom.Common.DTOs.Response;

public class StandingDto : IPlistSerializable
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int UserId { get; set; }
    public int GhostId { get; set; }
    public int Time { get; set; }
    public string HeroStyle { get; set; }
    public string WheelStyle { get; set; }
    public string EngineStyle { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public PlayerDto BoomUser { get; set; }
    public int Rank { get; set; }
    public bool IsSelf { get; set; }
}