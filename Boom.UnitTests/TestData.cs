using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;

namespace Boom.UnitTests;

public static class TestData
{
    public static GetScheduleDto GetScheduleDto =>
        new()
        {
            _id = "abc123",
            Badge = 42,
            CountryCode = "US",
            Email = "test@example.com",
            EngineStyle = "Sport",
            Fullname = "John Doe",
            HeroStyle = "Warrior",
            Level = 15,
            MaxGroupIdUnlocked = "group_7",
            Nickname = "Speedster",
            Notification = "Enabled",
            Timestamp = 1672531200000, // Unix timestamp in milliseconds
            Timezone = "America/New_York",
            TimezoneSecondsOffset = -18000, // UTC-5 in seconds
            TotalDistance = 1523.45,
            TotalEarnedMedals = 12,
            TotalEarnedSuperstars = 5,
            UserUuid = Guid.NewGuid(),
            WheelStyle = "Alloy"
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