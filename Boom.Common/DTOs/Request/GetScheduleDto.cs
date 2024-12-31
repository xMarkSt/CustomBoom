namespace Boom.Common.DTOs.Request;

public class GetScheduleDto
{
    public string? _id { get; set; }
    public int? badge { get; set; }
    public string? country_code { get; set; }
    public string? email { get; set; }
    public string? engine_style { get; set; }
    public string? fullname { get; set; }
    public string? hero_style { get; set; }
    public int? level { get; set; }
    public string? max_group_id_unlocked { get; set; }
    public string? nickname { get; set; }
    public string? notification { get; set; }
    public long? timestamp { get; set; }
    public string? timezone { get; set; }
    public int? timezone_seconds_offset { get; set; }
    public double? total_distance { get; set; }
    public int? total_earned_medals { get; set; }
    public int? total_earned_superstars { get; set; }
    public Guid user_uuid { get; set; }
    public string? wheel_style { get; set; }
}
