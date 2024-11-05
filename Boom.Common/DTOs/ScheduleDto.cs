namespace Boom.Common.DTOs;

public class ScheduleDto
{
    public List<TournamentGroupDto> Schedule { get; set; }
}

public class TournamentGroupDto
{
    public Guid Uuid { get; set; }
    public int LevelId { get; set; }
    public LevelDto Level { get; set; }
    public bool NoSuper { get; set; }
    public int SecondsToEnd { get; set; }
    public int SecondsToStart { get; set; }
}

public class LevelDto
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