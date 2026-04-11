using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;

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

    public static Player Player =>
        new()
        {
            Id = 99L,
            Uuid = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            FacebookId = 111222333L,
            TwitterId = 444555666L,
            Nickname = "Speedster",
            Fullname = "John Doe",
            Notification = "Enabled",
            Email = "test@example.com",
            Badge = 42,
            LastLoginAt = new DateTime(2024, 6, 15, 10, 30, 0, DateTimeKind.Utc),
            CountryCode = "US",
            Timezone = "America/New_York",
            TimezoneSecondsOffset = -18000,
            Rev = 7,
            Device = "iPhone14",
            Ios = "17.0",
            TinyUrl = "https://t.co/abc",
            HeroStyle = "Warrior",
            EngineStyle = "Sport",
            WheelStyle = "Alloy",
            TotalHiddenPilesFound = 3,
            TotalEarnedMedals = 12,
            TotalEarnedSuperstars = 5,
            TotalDistance = 1523.45,
            VsPlayed = 20,
            VsWon = 14,
            WcPlayed = 10,
            WcWon = 4,
            TournamentsAggregatedRank = 3,
            MaxGroupIdUnlocked = "group_7",
            CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
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