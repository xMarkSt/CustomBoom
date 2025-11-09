namespace Boom.Common.DTOs.Response;

public class TournamentGroupDto : IPlistSerializable
{
    public Guid Uuid { get; set; }
    public int LevelId { get; set; }
    public LevelTargetDto Level { get; set; }
    public int NoSuper { get; set; }
    public int SecondsToEnd { get; set; }
    public int SecondsToStart { get; set; }
}