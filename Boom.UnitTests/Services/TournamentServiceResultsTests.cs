using AutoMapper;
using Boom.Business.Services;
using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Boom.UnitTests.Services;

[TestFixture]
public class TournamentServiceResultsTests
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
    }

    [Test]
    public async Task Results_PlayerInPodium_ReturnsTop3WithoutDuplicate()
    {
        var tournamentUuid = Guid.NewGuid();
        var self = Standing(id: 2, userId: 99, time: 2000);
        var tournament = Tournament(tournamentUuid, endsAt: DateTime.UtcNow.AddHours(1),
            Standing(id: 1, userId: 50, time: 1000),
            self,
            Standing(id: 3, userId: 51, time: 3000));
        SetupTournaments(tournament);
        SetupStandings(tournament.Standings.ToArray());

        var result = await _service.Results(Dto(tournamentUuid), Player(99));

        var entry = result[tournamentUuid];
        entry.Rank.Should().Be(2);
        entry.Completed.Should().Be(0);
        entry.Standings.Should().HaveCount(3);
        entry.Standings.Should().ContainSingle(s => s.IsSelf);
        entry.Standings.First(s => s.IsSelf).Rank.Should().Be(2);
    }

    [Test]
    public async Task Results_PlayerOutsidePodium_AppendsSelfWithRealRank()
    {
        var tournamentUuid = Guid.NewGuid();
        var self = Standing(id: 4, userId: 99, time: 9000);
        var tournament = Tournament(tournamentUuid, endsAt: DateTime.UtcNow.AddHours(1),
            Standing(id: 1, userId: 50, time: 1000),
            Standing(id: 2, userId: 51, time: 2000),
            Standing(id: 3, userId: 52, time: 3000),
            self);
        SetupTournaments(tournament);
        SetupStandings(tournament.Standings.ToArray());

        var result = await _service.Results(Dto(tournamentUuid), Player(99));

        var entry = result[tournamentUuid];
        entry.Rank.Should().Be(4);
        entry.Standings.Should().HaveCount(4); // top 3 + self
        var selfDto = entry.Standings.Should().ContainSingle(s => s.IsSelf).Subject;
        selfDto.Rank.Should().Be(4);
    }

    [Test]
    public async Task Results_TournamentEnded_MarksCompleted()
    {
        var tournamentUuid = Guid.NewGuid();
        var tournament = Tournament(tournamentUuid, endsAt: DateTime.UtcNow.AddHours(-1),
            Standing(id: 1, userId: 99, time: 1000));
        SetupTournaments(tournament);
        SetupStandings(tournament.Standings.ToArray());

        var result = await _service.Results(Dto(tournamentUuid), Player(99));

        result[tournamentUuid].Completed.Should().Be(1);
    }

    [Test]
    public async Task Results_MultipleTournamentUuids_KeysResponseByEach()
    {
        var uuidA = Guid.NewGuid();
        var uuidB = Guid.NewGuid();
        var tournamentA = Tournament(uuidA, endsAt: DateTime.UtcNow.AddHours(1), Standing(id: 1, userId: 99, time: 1000));
        var tournamentB = Tournament(uuidB, endsAt: DateTime.UtcNow.AddHours(1), Standing(id: 2, userId: 99, time: 2000));
        SetupTournaments(tournamentA, tournamentB);
        SetupStandings(tournamentA.Standings.Concat(tournamentB.Standings).ToArray());

        var result = await _service.Results(Dto(uuidA, uuidB), Player(99));

        result.Keys.Should().BeEquivalentTo(new[] { uuidA, uuidB });
    }

    [Test]
    public async Task Results_UpdatesPlayerStats_PlayedAndWon()
    {
        // Player 99 won tournament A (fastest time) but not tournament B.
        var uuidA = Guid.NewGuid();
        var uuidB = Guid.NewGuid();
        var winningStanding = Standing(id: 1, userId: 99, time: 1000);
        var losingStanding = Standing(id: 3, userId: 99, time: 5000);
        var tournamentA = Tournament(uuidA, endsAt: DateTime.UtcNow.AddHours(1),
            winningStanding, Standing(id: 2, userId: 50, time: 2000));
        var tournamentB = Tournament(uuidB, endsAt: DateTime.UtcNow.AddHours(1),
            losingStanding, Standing(id: 4, userId: 50, time: 100));
        SetupTournaments(tournamentA, tournamentB);
        SetupStandings(tournamentA.Standings.Concat(tournamentB.Standings).ToArray());

        var player = Player(99);
        await _service.Results(Dto(uuidA, uuidB), player);

        player.WcPlayed.Should().Be(2);
        player.WcWon.Should().Be(1);
        _mockRepository.Verify(r => r.Update(player), Times.Once);
        _mockRepository.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task Results_TiedFastestTime_OnlyEarliestSubmittedStandingCountsAsWon()
    {
        // Players 99 and 50 have the exact same (fastest) time. Only whoever submitted first
        // (the lower Standing id) should be credited with the win, matching PHP's rank accessor.
        var tournamentUuid = Guid.NewGuid();
        var firstSubmitted = Standing(id: 1, userId: 99, time: 1000);
        var secondSubmitted = Standing(id: 2, userId: 50, time: 1000);
        var tournament = Tournament(tournamentUuid, endsAt: DateTime.UtcNow.AddHours(1),
            firstSubmitted, secondSubmitted);
        SetupTournaments(tournament);
        SetupStandings(tournament.Standings.ToArray());

        var player99 = Player(99);
        var result = await _service.Results(Dto(tournamentUuid), player99);

        result[tournamentUuid].Rank.Should().Be(1);
        player99.WcWon.Should().Be(1);

        // Re-run for the second-place-on-tiebreak player: they must NOT be credited with a win.
        var player50 = Player(50);
        await _service.Results(Dto(tournamentUuid), player50);
        player50.WcWon.Should().Be(0);
    }

    [Test]
    public async Task Results_PlayerHasNoStandingInTournament_ReturnsOnlyPodium()
    {
        var tournamentUuid = Guid.NewGuid();
        var tournament = Tournament(tournamentUuid, endsAt: DateTime.UtcNow.AddHours(1),
            Standing(id: 1, userId: 50, time: 1000));
        SetupTournaments(tournament);
        SetupStandings(tournament.Standings.ToArray());

        var result = await _service.Results(Dto(tournamentUuid), Player(99));

        var entry = result[tournamentUuid];
        entry.Rank.Should().Be(0);
        entry.Standings.Should().HaveCount(1);
        entry.Standings.Should().NotContain(s => s.IsSelf);
    }

    // --- Helpers ---

    private void SetupTournaments(params Tournament[] tournaments) =>
        _mockRepository.Setup(r => r.GetAll<Tournament>())
            .Returns(tournaments.AsQueryable().BuildMock());

    private void SetupStandings(params Standing[] standings) =>
        _mockRepository.Setup(r => r.GetAll<Standing>())
            .Returns(standings.AsQueryable().BuildMock());

    private static Player Player(long id) => new() { Id = id, Uuid = Guid.NewGuid() };

    private static long _nextTournamentId = 1;

    private static Tournament Tournament(Guid uuid, DateTime endsAt, params Standing[] standings)
    {
        var tournament = new Tournament
        {
            Id = _nextTournamentId++,
            Uuid = uuid,
            TournamentGroup = new TournamentGroup { EndsAt = endsAt },
            Standings = standings.ToList()
        };
        foreach (var standing in standings)
        {
            standing.TournamentId = tournament.Id;
        }
        return tournament;
    }

    private static Standing Standing(long id, long userId, int time) => new()
    {
        Id = id,
        UserId = userId,
        Time = time,
        HeroStyle = "h",
        WheelStyle = "w",
        EngineStyle = "e",
        Player = new Player { Id = userId, Uuid = Guid.NewGuid(), Nickname = $"P{userId}" }
    };

    private static GetTournamentResultsDto Dto(params Guid[] tournamentUuids) => new()
    {
        UserUuid = Guid.NewGuid(),
        TournamentUuids = tournamentUuids.ToList()
    };
}
