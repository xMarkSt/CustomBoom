namespace Boom.Common.DTOs.Response;

public class TournamentResultsDto : IPlistSerializable
{
    public int Completed { get; set; }
    public int Rank { get; set; }
    public List<StandingDto> Standings { get; set; } = new();
}
