namespace Boom.Common.DTOs;

public class ScheduleDto : IPlistSerializable
{
    public List<TournamentGroupDto> Schedule { get; set; }
}