using AutoMapper;
using Boom.Business.MappingProfiles;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;

namespace Boom.UnitTests;

[TestFixture]
public class MappingTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new PlayerProfile());
            cfg.AddProfile(new StandingProfile());
            cfg.AddProfile(new TournamentProfile());
        });
        _mapper = new Mapper(configuration);
    }

    [Test]
    public void Test_GetScheduleDto_Player_Mapping()
    {
        var testData = TestData.GetScheduleDto;

        var mapped = _mapper.Map<Player>(testData);

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

    [Test]
    public void Test_Player_PlayerDto_Mapping()
    {
        var player = TestData.Player;

        var dto = _mapper.Map<PlayerDto>(player);

        dto.Id.Should().Be((int)player.Id);
        dto.Uuid.Should().Be(player.Uuid);
        dto.FacebookId.Should().Be(player.FacebookId.ToString());
        dto.TwitterId.Should().Be(player.TwitterId.ToString());
        dto.Nickname.Should().Be(player.Nickname);
        dto.Fullname.Should().Be(player.Fullname);
        dto.Notification.Should().Be(player.Notification);
        dto.Email.Should().Be(player.Email);
        dto.Badge.Should().Be(player.Badge);
        dto.LastLoginAt.Should().Be(player.LastLoginAt!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        dto.CountryCode.Should().Be(player.CountryCode);
        dto.Timezone.Should().Be(player.Timezone);
        dto.TimezoneSecondsOffset.Should().Be(player.TimezoneSecondsOffset);
        dto.Rev.Should().Be(player.Rev!.Value);
        dto.Device.Should().Be(player.Device);
        dto.Ios.Should().Be(player.Ios);
        dto.CreatedAt.Should().Be(player.CreatedAt!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        dto.UpdatedAt.Should().Be(player.UpdatedAt!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        dto.TinyUrl.Should().Be(player.TinyUrl);
        dto.HeroStyle.Should().Be(player.HeroStyle);
        dto.EngineStyle.Should().Be(player.EngineStyle);
        dto.WheelStyle.Should().Be(player.WheelStyle);
        dto.TotalHiddenPilesFound.Should().Be(player.TotalHiddenPilesFound!.Value);
        dto.TournamentsAggregatedRank.Should().Be(player.TournamentsAggregatedRank);
    }

    [Test]
    public void Test_Player_PlayerProfileDto_Mapping()
    {
        var player = TestData.Player;

        var dto = _mapper.Map<PlayerDto>(player);

        dto.Profile.Should().NotBeNull();
        dto.Profile.HeroStyle.Should().Be(player.HeroStyle);
        dto.Profile.EngineStyle.Should().Be(player.EngineStyle);
        dto.Profile.WheelStyle.Should().Be(player.WheelStyle);
        dto.Profile.Nickname.Should().Be(player.Nickname);
        dto.Profile.CountryCode.Should().Be(player.CountryCode);
        dto.Profile.TotalEarnedMedals.Should().Be(player.TotalEarnedMedals);
        dto.Profile.TotalEarnedSuperstars.Should().Be(player.TotalEarnedSuperstars);
        dto.Profile.TotalDistance.Should().Be(player.TotalDistance);
        dto.Profile.MaxGroupIdUnlocked.Should().Be(player.MaxGroupIdUnlocked);
    }

    [Test]
    public void Test_Player_ChallengeStatsDto_Mapping()
    {
        var player = TestData.Player;

        var dto = _mapper.Map<PlayerDto>(player);

        dto.Profile.ChallengeStats.Should().NotBeNull();
        dto.Profile.ChallengeStats.Won.Should().Be(player.VsWon);
        dto.Profile.ChallengeStats.Played.Should().Be(player.VsPlayed);
        dto.Profile.ChallengeStats.Lost.Should().Be(player.VsPlayed - player.VsWon);
    }

    [Test]
    public void Test_Player_TournamentStatsDto_Mapping()
    {
        var player = TestData.Player;

        var dto = _mapper.Map<PlayerDto>(player);

        dto.Profile.TournamentStats.Should().NotBeNull();
        dto.Profile.TournamentStats.Won.Should().Be(player.WcWon);
        dto.Profile.TournamentStats.Played.Should().Be(player.WcPlayed);
        dto.Profile.TournamentStats.AveragePos.Should().Be(0);
    }

    [Test]
    public void Test_Standing_StandingDto_Mapping()
    {
        var standing = TestData.Standing;

        var dto = _mapper.Map<StandingDto>(standing);

        dto.Id.Should().Be((int)standing.Id);
        dto.TournamentId.Should().Be((int)standing.TournamentId);
        dto.UserId.Should().Be((int)standing.UserId);
        dto.GhostId.Should().Be((int)standing.GhostId);
        dto.Time.Should().Be(standing.Time);
        dto.HeroStyle.Should().Be(standing.HeroStyle);
        dto.WheelStyle.Should().Be(standing.WheelStyle);
        dto.EngineStyle.Should().Be(standing.EngineStyle);
        dto.CreatedAt.Should().Be(standing.CreatedAt!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        dto.UpdatedAt.Should().Be(standing.UpdatedAt!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        dto.Rank.Should().Be(0);
        dto.IsSelf.Should().Be(false);
    }

    [Test]
    public void Test_Tournament_TournamentDto_Mapping()
    {
        var tournament = TestData.Tournament;

        var dto = _mapper.Map<TournamentDto>(tournament);

        dto.Id.Should().Be((int)tournament.Id);
        dto.TournamentGroupId.Should().Be((int)tournament.TournamentGroupId);
        dto.Uuid.Should().Be(tournament.Uuid);
        dto.EloGroup.Should().Be(tournament.EloGroup);
        dto.Cheaters.Should().Be(tournament.Cheaters);
        dto.Users.Should().Be(tournament.Standings.Count);
        dto.CreatedAt.Should().Be(tournament.CreatedAt!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        dto.UpdatedAt.Should().Be(tournament.UpdatedAt!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    [Test]
    public void Test_Standing_BoomUser_Mapping()
    {
        var standing = TestData.Standing;

        var dto = _mapper.Map<StandingDto>(standing);

        dto.BoomUser.Should().NotBeNull();
        dto.BoomUser.Id.Should().Be((int)standing.Player.Id);
        dto.BoomUser.Uuid.Should().Be(standing.Player.Uuid);
        dto.BoomUser.Nickname.Should().Be(standing.Player.Nickname);
    }
}