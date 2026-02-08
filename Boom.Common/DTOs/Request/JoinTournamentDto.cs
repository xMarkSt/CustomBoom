using Boom.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boom.Common.DTOs.Request
{
    public class JoinTournamentDto : IPlayerInfo
    {
        [FromForm(Name = "_id")]
        public string? Id { get; set; }

        [FromForm(Name = "badge")]
        public int? Badge { get; set; }

        [FromForm(Name = "compressed")]
        public int? Compressed { get; set; }

        [FromForm(Name = "country_code")]
        public string? CountryCode { get; set; }

        [FromForm(Name = "email")]
        public string? Email { get; set; }

        [FromForm(Name = "engine_style")]
        public string? EngineStyle { get; set; }

        [FromForm(Name = "fullname")]
        public string? FullName { get; set; }

        [FromForm(Name = "group_uuid")]
        public Guid? GroupUuid { get; set; }

        [FromForm(Name = "hero_style")]
        public string? HeroStyle { get; set; }

        [FromForm(Name = "max_group_id_unlocked")]
        public string? MaxGroupIdUnlocked { get; set; }

        [FromForm(Name = "nickname")]
        public string? Nickname { get; set; }

        [FromForm(Name = "notification")]
        public string? Notification { get; set; }

        [FromForm(Name = "time")]
        public long? Time { get; set; }

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

        [FromForm(Name = "uuid")]
        public Guid Uuid { get; set; }

        [FromForm(Name = "wheel_style")]
        public string? WheelStyle { get; set; }

        // Placeholder for the binary payload
        [FromForm(Name = "ghostData")]
        public IFormFile? GhostData { get; set; }
    }
}
