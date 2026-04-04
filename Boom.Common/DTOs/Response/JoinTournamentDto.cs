namespace Boom.Common.DTOs.Response;

public class JoinTournamentDto : IPlistSerializable
{
    public TournamentDto Tournament { get; set; }
    public int Rank { get; set; } // TODO get from standings rank?
    public List<StandingDto> Standings { get; set; }
}