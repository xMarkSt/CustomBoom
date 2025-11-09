namespace Boom.Common.DTOs.Response;

public class LevelTargetDto : IPlistSerializable
{
    public string ThemeName { get; set; }
    public string LevelName { get; set; }
    public string LevelId { get; set; }
    public int Version { get; set; }
    public string Target { get; set; }
    public bool Online { get; set; }
    public string Url { get; set; }
    [PlistPropertyName("bgName")] 
    public string BgName { get; set; }
}