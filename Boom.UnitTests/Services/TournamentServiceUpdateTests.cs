using AutoMapper;
using Boom.Business.Services;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;
using MockQueryable;
using Moq;
using UpdateTournamentDto = Boom.Common.DTOs.Request.UpdateTournamentDto;
using static Boom.UnitTests.Services.TournamentTestFixtures;

namespace Boom.UnitTests.Services;

[TestFixture]
public class TournamentServiceUpdateTests
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
        // (Rank/IsSelf are set by the service after mapping, from the entity.)
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

    // --- Not found -> null (=> 404 at the controller) ---

    [Test]
    public async Task Update_TournamentNotFound_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetAll<Tournament>())
            .Returns(new List<Tournament>().AsQueryable().BuildMock());

        var result = await _service.Update(Dto(Guid.NewGuid(), time: 1000), Player(99));

        result.Should().BeNull();
        _mockRepository.Verify(r => r.Update(It.IsAny<Standing>()), Times.Never);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Never);
    }

    [Test]
    public async Task Update_PlayerHasNoStanding_ReturnsNull()
    {
        var uuid = Guid.NewGuid();
        var tournament = Tournament(uuid, Standing(id: 1, userId: 50, time: 3000, ghostId: 10));
        SetupTournament(tournament);

        // Player 99 never joined this tournament.
        var result = await _service.Update(Dto(uuid, time: 1000), Player(99));

        result.Should().BeNull();
        _mockRepository.Verify(r => r.Update(It.IsAny<Standing>()), Times.Never);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Never);
    }

    // --- Best-or-equal-time overwrite ---

    [Test]
    public async Task Update_DoesNotOverwrite_WhenNewTimeIsWorse()
    {
        var uuid = Guid.NewGuid();
        var standing = Standing(id: 7, userId: 99, time: 3000, ghostId: 15);
        SetupTournament(Tournament(uuid, standing));

        // New time 9000 is SLOWER than the existing 3000 — leave the standing untouched.
        var result = await _service.Update(
            Dto(uuid, time: 9000, ghost: new byte[] { 1, 2, 3 }, hero: "h2", engine: "e2", wheel: "w2"),
            Player(99));

        standing.Time.Should().Be(3000);
        standing.HeroStyle.Should().Be("old");
        standing.EngineStyle.Should().Be("old");
        standing.WheelStyle.Should().Be("old");
        _mockRepository.Verify(r => r.Remove(It.IsAny<Ghost>()), Times.Never);
        _mockRepository.Verify(r => r.Update(It.IsAny<Standing>()), Times.Never);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Never);
        // Still returns the current standings (not null).
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Update_OverwritesTimeAndStyles_WhenNewTimeIsBetter()
    {
        var uuid = Guid.NewGuid();
        var standing = Standing(id: 7, userId: 99, time: 3000, ghostId: 15);
        SetupTournament(Tournament(uuid, standing));
        _mockRepository.Setup(r => r.GetById<Ghost>(15L)).Returns(new Ghost { Data = new byte[] { 0 } });

        await _service.Update(
            Dto(uuid, time: 1000, ghost: new byte[] { 1, 2, 3 }, hero: "h2", engine: "e2", wheel: "w2"),
            Player(99));

        standing.Time.Should().Be(1000);
        standing.HeroStyle.Should().Be("h2");
        standing.EngineStyle.Should().Be("e2");
        standing.WheelStyle.Should().Be("w2");
        _mockRepository.Verify(r => r.Update(standing), Times.Once);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Update_DoesNotOverwrite_WhenNewTimeIsEqual()
    {
        var uuid = Guid.NewGuid();
        var standing = Standing(id: 7, userId: 99, time: 3000, ghostId: 15);
        SetupTournament(Tournament(uuid, standing));

        // Equal time is not a strictly-better run: the ghost (and its embedded styles) must stay
        // paired with the recorded time, so nothing is overwritten.
        var result = await _service.Update(
            Dto(uuid, time: 3000, ghost: new byte[] { 7, 8 }, hero: "h2", engine: "e2", wheel: "w2"),
            Player(99));

        standing.Time.Should().Be(3000);
        standing.HeroStyle.Should().Be("old");
        standing.EngineStyle.Should().Be("old");
        standing.WheelStyle.Should().Be("old");
        _mockRepository.Verify(r => r.Remove(It.IsAny<Ghost>()), Times.Never);
        _mockRepository.Verify(r => r.Update(It.IsAny<Standing>()), Times.Never);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Never);
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Update_ReplacesGhost_RemovesOldAndAssignsNewData()
    {
        var uuid = Guid.NewGuid();
        var standing = Standing(id: 7, userId: 99, time: 3000, ghostId: 15);
        SetupTournament(Tournament(uuid, standing));
        var oldGhost = new Ghost { Data = new byte[] { 9, 9, 9 } };
        _mockRepository.Setup(r => r.GetById<Ghost>(15L)).Returns(oldGhost);

        var newData = new byte[] { 1, 2, 3, 4 };
        await _service.Update(Dto(uuid, time: 1000, ghost: newData), Player(99));

        _mockRepository.Verify(r => r.Remove(oldGhost), Times.Once);
        standing.Ghost.Should().NotBeSameAs(oldGhost);
        standing.Ghost.Data.Should().BeEquivalentTo(newData);
    }

    // --- Standings response / ranking (reuses BuildJoinResponse) ---

    [Test]
    public async Task Update_ReturnsStandings_WithPlayerRank_WhenNotFastest()
    {
        var uuid = Guid.NewGuid();
        var leader = Standing(id: 1, userId: 50, time: 1000, ghostId: 10);
        var self = Standing(id: 2, userId: 99, time: 3000, ghostId: 20);
        SetupTournament(Tournament(uuid, leader, self));
        _mockRepository.Setup(r => r.GetById<Ghost>(It.IsAny<long>())).Returns(new Ghost { Data = new byte[] { 0 } });

        // Improve self to 2000 (strictly better than 3000) but still behind the leader (1000).
        var result = await _service.Update(Dto(uuid, time: 2000), Player(99));

        result.Should().NotBeNull();
        result!.Rank.Should().Be(2);
        // Two real standings + the rank-0 copy of #1 that BuildJoinResponse prepends.
        result.Standings.Should().HaveCount(3);
        result.Standings[0].Rank.Should().Be(0);
    }

    [Test]
    public async Task Update_PlayerBecomesFastest_ReportsRank1()
    {
        var uuid = Guid.NewGuid();
        var leader = Standing(id: 1, userId: 50, time: 1000, ghostId: 10);
        var self = Standing(id: 2, userId: 99, time: 3000, ghostId: 20);
        SetupTournament(Tournament(uuid, leader, self));
        _mockRepository.Setup(r => r.GetById<Ghost>(It.IsAny<long>())).Returns(new Ghost { Data = new byte[] { 0 } });

        // Beat the leader (500 < 1000) — this is the rank-#1 branch (Discord notification hook).
        var result = await _service.Update(Dto(uuid, time: 500), Player(99));

        result.Should().NotBeNull();
        result!.Rank.Should().Be(1);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Update_TiedFastestTime_EarliestSubmittedStandingRanksFirst()
    {
        // Two standings tie for the fastest time. Whoever submitted first (lower id) must rank
        // above the other, matching PHP's Standing::rank accessor (ordered by time, ties broken
        // by row order).
        var uuid = Guid.NewGuid();
        var earlier = Standing(id: 1, userId: 50, time: 1000, ghostId: 10);
        var later = Standing(id: 2, userId: 51, time: 1000, ghostId: 11);
        var self = Standing(id: 3, userId: 99, time: 3000, ghostId: 20);
        SetupTournament(Tournament(uuid, earlier, later, self));

        // Equal time leaves self's own standing untouched; this only exercises the ranking display.
        var result = await _service.Update(Dto(uuid, time: 3000), Player(99));

        result.Should().NotBeNull();
        result!.Standings.Should().ContainSingle(s => s.Id == (int)earlier.Id && s.Rank == 1);
        result.Standings.Should().ContainSingle(s => s.Id == (int)later.Id && s.Rank == 2);
    }

    // --- Helpers ---

    private void SetupTournament(Tournament tournament) =>
        _mockRepository.Setup(r => r.GetAll<Tournament>())
            .Returns(new List<Tournament> { tournament }.AsQueryable().BuildMock());

    private static UpdateTournamentDto Dto(
        Guid tournamentUuid, int time, byte[]? ghost = null,
        string hero = "h", string engine = "e", string wheel = "w") => new()
    {
        TournamentUuid = tournamentUuid,
        UserUuid = Guid.NewGuid(),
        Time = time,
        HeroStyle = hero,
        EngineStyle = engine,
        WheelStyle = wheel,
        GhostData = GhostFile(ghost ?? new byte[] { 1 })
    };
}
