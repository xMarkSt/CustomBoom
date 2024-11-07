using Boom.Business.Services;

namespace Boom.Common.DTOs;

public class ScheduleDto: IPlistSerializable
{
    public List<TournamentGroupDto> Schedule { get; set; }
}
// TODO: Check entities to know which are nullable
public class TournamentGroupDto: IPlistSerializable
{
    public Guid Uuid { get; set; }
    public int LevelId { get; set; }
    public LevelDto Level { get; set; }
    public bool NoSuper { get; set; }
    public int SecondsToEnd { get; set; }
    public int SecondsToStart { get; set; }
}

public class LevelDto: IPlistSerializable
{
    public string ThemeName { get; set; }
    public string LevelName { get; set; }
    public string LevelId { get; set; }
    public int Version { get; set; }
    public string Target { get; set; }
    public bool Online { get; set; }
    public string Url { get; set; }
    public string BgName { get; set; }
}