using AutoMapper;
using Boom.Business.MappingProfiles;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;

namespace Boom.UnitTests;

[TestFixture]
public class MappingTests
{
    [Test]
    public void Test_GetScheduleDto_Player_Mapping()
    {
        // Arrange
        var profile = new PlayerProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
        var mapper = new Mapper(configuration);

        var testData = TestData.GetScheduleDto;

        // Act
        var mapped = mapper.Map<Player>(testData);

        mapped.Badge.Should().Be(testData.badge);
        mapped.CountryCode.Should().Be(testData.country_code);
        mapped.Email.Should().Be(testData.email);
        mapped.EngineStyle.Should().Be(testData.engine_style);
        mapped.Fullname.Should().Be(testData.fullname);
        mapped.HeroStyle.Should().Be(testData.hero_style);
        // mapped.Level.Should().Be(testData.level);
        mapped.MaxGroupIdUnlocked.Should().Be(testData.max_group_id_unlocked);
        mapped.Nickname.Should().Be(testData.nickname);
        mapped.Notification.Should().Be(testData.notification);
        // mapped.Timestamp.Should().Be(testData.timestamp);
        mapped.Timezone.Should().Be(testData.timezone);
        mapped.TimezoneSecondsOffset.Should().Be(testData.timezone_seconds_offset);
        mapped.TotalDistance.Should().Be(testData.total_distance);
        mapped.TotalEarnedMedals.Should().Be(testData.total_earned_medals);
        mapped.TotalEarnedSuperstars.Should().Be(testData.total_earned_superstars);
        mapped.Uuid.Should().Be(testData.user_uuid);
        mapped.WheelStyle.Should().Be(testData.wheel_style);
    }
}