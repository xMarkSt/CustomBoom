using Boom.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boom.Common.DTOs.Request;

public class GetScheduleDto : IPlayerInfo
{
    [FromForm(Name = "_id")]
    public string? _id { get; set; }
    
    [FromForm(Name = "badge")]
    public int? Badge { get; set; }
    
    [FromForm(Name = "country_code")]
    public string? CountryCode { get; set; }
    
    [FromForm(Name = "email")]
    public string? Email { get; set; }
    
    [FromForm(Name = "engine_style")]
    public string? EngineStyle { get; set; }
    
    [FromForm(Name = "fullname")]
    public string? Fullname { get; set; }
    
    [FromForm(Name = "hero_style")]
    public string? HeroStyle { get; set; }
    
    [FromForm(Name = "level")]
    public int? Level { get; set; }
    
    [FromForm(Name = "max_group_id_unlocked")]
    public string? MaxGroupIdUnlocked { get; set; }
    
    [FromForm(Name = "nickname")]
    public string? Nickname { get; set; }
    
    [FromForm(Name = "notification")]
    public string? Notification { get; set; }
    
    [FromForm(Name = "timestamp")]
    public long? Timestamp { get; set; }
    
    [FromForm(Name = "timezone")]
    public string? Timezone { get; set; }
    
    [FromForm(Name = "timezone_seconds_offset")]
    public int? TimezoneSecondsOffset { get; set; }
    
    [FromForm(Name = "total_distance")]
    public double? TotalDistance { get; set; }
    
    [FromForm(Name = "total_earned_medals")]
    public int? TotalEarnedMedals { get; set; }
    
    [FromForm(Name = "total_earned_superstars")]
    public int? TotalEarnedSuperstars { get; set; }
    
    [FromForm(Name = "user_uuid")]
    public Guid UserUuid { get; set; }
    
    [FromForm(Name = "wheel_style")]
    public string? WheelStyle { get; set; }
}
