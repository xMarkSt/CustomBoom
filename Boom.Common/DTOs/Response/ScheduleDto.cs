namespace Boom.Common.DTOs.Response;

public class ScheduleDto : IPlistSerializable
{
    public List<TournamentGroupDto> Schedule { get; set; }
}