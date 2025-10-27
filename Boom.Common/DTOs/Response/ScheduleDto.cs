namespace Boom.Common.DTOs.Response;

public class ScheduleDto : IPlistSerializable
{
    public List<TournamentGroupDto> Schedule { get; set; }
    
    [PlistPropertyName("_sk")]
    public string? SecretKey { get; set; }
}
