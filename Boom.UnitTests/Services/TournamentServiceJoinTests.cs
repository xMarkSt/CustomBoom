using AutoMapper;
using Boom.Business.Services;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;
using MockQueryable;
using Moq;
using JoinTournamentDto = Boom.Common.DTOs.Request.JoinTournamentDto;
using static Boom.UnitTests.Services.TournamentTestFixtures;

namespace Boom.UnitTests.Services;

[TestFixture]
public class TournamentServiceJoinTests
{
    private Mock<IRepository> _mockRepository = null!;
    private Mock<IMapper> _mockMapper = null!;
    private TournamentService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new TournamentService(_mockRepository.Object, _mockMapper.Object);

        // Echo each Standing into a StandingDto so BuildJoinResponse can rank them.
        _mockMapper.Setup(m => m.Map<StandingDto>(It.IsAny<Standing>()))
            .Returns((Standing s) => new StandingDto
            {
                Id = (int)s.Id,
                UserId = (int)s.UserId,
                Time = s.Time,
                HeroStyle = s.HeroStyle,
                WheelStyle = s.WheelStyle,
                EngineStyle = s.EngineStyle,
            });
        _mockMapper.Setup(m => m.Map<TournamentDto>(It.IsAny<Tournament>()))
            .Returns((Tournament t) => new TournamentDto { Id = (int)t.Id, Uuid = t.Uuid });
    }

    // --- Not found / ended -> null ---

    [Test]
    public async Task Join_GroupNotFound_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetAll<TournamentGroup>())
            .Returns(new List<TournamentGroup>().AsQueryable().BuildMock());

        var result = await _service.Join(Dto(Guid.NewGuid(), time: 1000), Player(99));

        result.Should().BeNull();
        _mockRepository.Verify(r => r.SaveAsync(), Times.Never);
    }

    [Test]
    public async Task Join_GroupHasEnded_ReturnsNull()
    {
        var uuid = Guid.NewGuid();
        var group = Group(uuid, endsAt: DateTime.UtcNow.AddMinutes(-1));
        SetupGroup(group);

        var result = await _service.Join(Dto(uuid, time: 1000), Player(99));

        result.Should().BeNull();
        _mockRepository.Verify(r => r.SaveAsync(), Times.Never);
    }

    // --- Not a personal best -> no save, existing standings returned ---

    [Test]
    public async Task Join_TimeNotABetterThanExisting_ReturnsStandingsWithoutSaving()
    {
        var uuid = Guid.NewGuid();
        var standing = Standing(id: 7, userId: 99, time: 3000, ghostId: 15);
        var tournament = Tournament(standing);
        var group = Group(uuid, endsAt: DateTime.UtcNow.AddHours(1), tournament);
        SetupGroup(group);

        // New time 9000 is SLOWER than existing 3000 - no update.
        var result = await _service.Join(Dto(uuid, time: 9000), Player(99));

        result.Should().NotBeNull();
        standing.Time.Should().Be(3000);
        _mockRepository.Verify(r => r.Update(It.IsAny<Standing>()), Times.Never);
        _mockRepository.Verify(r => r.Add(It.IsAny<Standing>()), Times.Never);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Never);
    }

    // --- New standing + ghost creation ---

    [Test]
    public async Task Join_PlayerHasNoExistingStanding_CreatesNewStandingAndGhost()
    {
        var uuid = Guid.NewGuid();
        var tournament = Tournament();
        var group = Group(uuid, endsAt: DateTime.UtcNow.AddHours(1), tournament);
        SetupGroup(group);

        var ghostBytes = new byte[] { 1, 2, 3, 4 };
        var player = Player(99);
        var result = await _service.Join(Dto(uuid, time: 1500, ghost: ghostBytes), player);

        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.Add(It.Is<Standing>(s =>
            s.UserId == 99 && s.Time == 1500 && Enumerable.SequenceEqual(s.Ghost.Data, ghostBytes))), Times.Once);
        _mockRepository.Verify(r => r.Update(It.IsAny<Standing>()), Times.Never);
        _mockRepository.Verify(r => r.Remove(It.IsAny<Ghost>()), Times.Never);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Once);
    }

    // --- Update existing standing + ghost replacement ---

    [Test]
    public async Task Join_TimeIsNewPersonalBest_UpdatesStandingAndReplacesGhost()
    {
        var uuid = Guid.NewGuid();
        var standing = Standing(id: 7, userId: 99, time: 3000, ghostId: 15);
        var tournament = Tournament(standing);
        var group = Group(uuid, endsAt: DateTime.UtcNow.AddHours(1), tournament);
        SetupGroup(group);

        var oldGhost = new Ghost { Data = new byte[] { 9, 9, 9 } };
        _mockRepository.Setup(r => r.GetById<Ghost>(15L)).Returns(oldGhost);

        var newData = new byte[] { 1, 2, 3, 4 };
        await _service.Join(Dto(uuid, time: 1000, ghost: newData), Player(99));

        standing.Time.Should().Be(1000);
        _mockRepository.Verify(r => r.Remove(oldGhost), Times.Once);
        standing.Ghost.Should().NotBeSameAs(oldGhost);
        standing.Ghost.Data.Should().BeEquivalentTo(newData);
        _mockRepository.Verify(r => r.Update(standing), Times.Once);
        _mockRepository.Verify(r => r.Add(It.IsAny<Standing>()), Times.Never);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Once);
    }

    // --- Ranking / IsSelf / rank-0 duplicate ---

    [Test]
    public async Task Join_StandingsAreSortedAscendingByTime_WithCorrectRanks()
    {
        var uuid = Guid.NewGuid();
        var first = Standing(id: 1, userId: 10, time: 1000, ghostId: 1);
        var second = Standing(id: 2, userId: 20, time: 2000, ghostId: 2);
        var self = Standing(id: 3, userId: 99, time: 3000, ghostId: 3);
        var tournament = Tournament(first, second, self);
        var group = Group(uuid, endsAt: DateTime.UtcNow.AddHours(1), tournament);
        SetupGroup(group);

        // Not a personal best (still slower than nothing above) - just reload/reads response.
        var result = await _service.Join(Dto(uuid, time: 9000), Player(99));

        result.Should().NotBeNull();
        // Rank-0 duplicate + 3 real standings, ranks 1..3 ascending by time.
        result.Standings.Should().HaveCount(4);
        result.Standings[0].Rank.Should().Be(0);
        result.Standings[1].Rank.Should().Be(1);
        result.Standings[1].UserId.Should().Be(10);
        result.Standings[2].Rank.Should().Be(2);
        result.Standings[2].UserId.Should().Be(20);
        result.Standings[3].Rank.Should().Be(3);
        result.Standings[3].UserId.Should().Be(99);
    }

    [Test]
    public async Task Join_SubmittingPlayersStanding_HasIsSelfTrue()
    {
        var uuid = Guid.NewGuid();
        var leader = Standing(id: 1, userId: 50, time: 1000, ghostId: 10);
        var self = Standing(id: 2, userId: 99, time: 3000, ghostId: 20);
        var tournament = Tournament(leader, self);
        var group = Group(uuid, endsAt: DateTime.UtcNow.AddHours(1), tournament);
        SetupGroup(group);

        var result = await _service.Join(Dto(uuid, time: 9000), Player(99));

        result.Should().NotBeNull();
        var selfDto = result.Standings.Single(s => s.UserId == 99 && s.Rank > 0);
        selfDto.IsSelf.Should().BeTrue();
        result.Standings.Single(s => s.UserId == 50 && s.Rank > 0).IsSelf.Should().BeFalse();
    }

    [Test]
    public async Task Join_Rank0DuplicateOfFirst_IsPrependedToStandings()
    {
        var uuid = Guid.NewGuid();
        var leader = Standing(id: 1, userId: 50, time: 1000, ghostId: 10);
        var self = Standing(id: 2, userId: 99, time: 3000, ghostId: 20);
        var tournament = Tournament(leader, self);
        var group = Group(uuid, endsAt: DateTime.UtcNow.AddHours(1), tournament);
        SetupGroup(group);

        var result = await _service.Join(Dto(uuid, time: 9000), Player(99));

        result.Should().NotBeNull();
        result.Standings[0].Rank.Should().Be(0);
        result.Standings[0].UserId.Should().Be(50); // duplicate of rank #1 (the leader)
        result.Standings[0].Time.Should().Be(1000);
    }

    // --- Helpers ---

    private void SetupGroup(TournamentGroup group) =>
        _mockRepository.Setup(r => r.GetAll<TournamentGroup>())
            .Returns(new List<TournamentGroup> { group }.AsQueryable().BuildMock());

    private static TournamentGroup Group(Guid uuid, DateTime endsAt, params Tournament[] tournaments) => new()
    {
        Id = 3,
        Uuid = uuid,
        EndsAt = endsAt,
        Tournaments = tournaments.ToList()
    };

    private static JoinTournamentDto Dto(
        Guid groupUuid, int time, byte[]? ghost = null,
        string hero = "h", string engine = "e", string wheel = "w") => new()
    {
        GroupUuid = groupUuid,
        UserUuid = Guid.NewGuid(),
        Time = time,
        HeroStyle = hero,
        
        EngineStyle = engine,
        WheelStyle = wheel,
        GhostData = GhostFile(ghost ?? new byte[] { 1 })
    };
}
