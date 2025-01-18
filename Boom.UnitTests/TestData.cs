using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;

namespace Boom.UnitTests;

public static class TestData
{
    public static GetScheduleDto GetScheduleDto =>
        new()
        {
            _id = "abc123",
            badge = 42,
            country_code = "US",
            email = "test@example.com",
            engine_style = "Sport",
            fullname = "John Doe",
            hero_style = "Warrior",
            level = 15,
            max_group_id_unlocked = "group_7",
            nickname = "Speedster",
            notification = "Enabled",
            timestamp = 1672531200000, // Unix timestamp in milliseconds
            timezone = "America/New_York",
            timezone_seconds_offset = -18000, // UTC-5 in seconds
            total_distance = 1523.45,
            total_earned_medals = 12,
            total_earned_superstars = 5,
            user_uuid = Guid.NewGuid(),
            wheel_style = "Alloy"
        };

    public static TournamentGroupDto TournamentGroupDto =>
        new()
        {
            Uuid = new Guid("800c4278-633d-45ac-9d97-4aad5950be6b"),
            LevelId = 61,
            Level = new LevelTargetDto
            {
                ThemeName = "Frozen Hills",
                LevelName = "Waterslide",
                LevelId = "Waterslide:0",
                Version = 1,
                Target = "", // Only filled for online levels
                Online = false,
                Url = "", // Only filled for online levels
                BgName = "NorthBG.plist"
            },
            NoSuper = 0,
            SecondsToEnd = 69164,
            SecondsToStart = -17236
        };
}