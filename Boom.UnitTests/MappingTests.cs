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

        mapped.Badge.Should().Be(testData.Badge);
        mapped.CountryCode.Should().Be(testData.CountryCode);
        mapped.Email.Should().Be(testData.Email);
        mapped.EngineStyle.Should().Be(testData.EngineStyle);
        mapped.Fullname.Should().Be(testData.Fullname);
        mapped.HeroStyle.Should().Be(testData.HeroStyle);
        // mapped.Level.Should().Be(testData.level);
        mapped.MaxGroupIdUnlocked.Should().Be(testData.MaxGroupIdUnlocked);
        mapped.Nickname.Should().Be(testData.Nickname);
        mapped.Notification.Should().Be(testData.Notification);
        // mapped.Timestamp.Should().Be(testData.timestamp);
        mapped.Timezone.Should().Be(testData.Timezone);
        mapped.TimezoneSecondsOffset.Should().Be(testData.TimezoneSecondsOffset);
        mapped.TotalDistance.Should().Be(testData.TotalDistance);
        mapped.TotalEarnedMedals.Should().Be(testData.TotalEarnedMedals);
        mapped.TotalEarnedSuperstars.Should().Be(testData.TotalEarnedSuperstars);
        mapped.Uuid.Should().Be(testData.UserUuid);
        mapped.WheelStyle.Should().Be(testData.WheelStyle);
    }
}