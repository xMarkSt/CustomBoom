using AutoMapper;
using Boom.Business.Services;
using Boom.Common.DTOs.Request;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Boom.UnitTests.Services
{
    [TestFixture]
    public class TournamentServiceGhostTests
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private TournamentService _tournamentService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _mockMapper = new Mock<IMapper>();
            _tournamentService = new TournamentService(_mockRepository.Object, _mockMapper.Object);
        }

        private void SetupStandings(IEnumerable<Standing> standings)
        {
            _mockRepository.Setup(r => r.GetAll<Standing>())
                .Returns(standings.AsQueryable().BuildMock());
        }

        [Test]
        public async Task GetGhost_OpponentStandingExists_ReturnsGhostDataAsIs()
        {
            // Arrange
            var tournamentUuid = Guid.NewGuid();
            var opponentUuid = Guid.NewGuid();
            var ghostBytes = new byte[] { 1, 2, 3, 4, 5 };

            var standing = new Standing
            {
                Tournament = new Tournament { Uuid = tournamentUuid },
                Player = new Player { Uuid = opponentUuid },
                Ghost = new Ghost { Data = ghostBytes }
            };
            SetupStandings(new[] { standing });

            var dto = new GhostTournamentDto { TournamentUuid = tournamentUuid, OpponentUuid = opponentUuid };

            // Act
            var result = await _tournamentService.GetGhost(dto);

            // Assert: bytes returned verbatim (no decompression) to match the original PHP behaviour
            result.Should().BeSameAs(ghostBytes);
        }

        [Test]
        public async Task GetGhost_TournamentNotFound_ReturnsNull()
        {
            // Arrange
            var standing = new Standing
            {
                Tournament = new Tournament { Uuid = Guid.NewGuid() },
                Player = new Player { Uuid = Guid.NewGuid() },
                Ghost = new Ghost { Data = new byte[] { 1 } }
            };
            SetupStandings(new[] { standing });

            var dto = new GhostTournamentDto { TournamentUuid = Guid.NewGuid(), OpponentUuid = Guid.NewGuid() };

            // Act
            var result = await _tournamentService.GetGhost(dto);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetGhost_OpponentNotInTournament_ReturnsNull()
        {
            // Arrange
            var tournamentUuid = Guid.NewGuid();
            var standing = new Standing
            {
                Tournament = new Tournament { Uuid = tournamentUuid },
                Player = new Player { Uuid = Guid.NewGuid() }, // different opponent
                Ghost = new Ghost { Data = new byte[] { 1 } }
            };
            SetupStandings(new[] { standing });

            var dto = new GhostTournamentDto { TournamentUuid = tournamentUuid, OpponentUuid = Guid.NewGuid() };

            // Act
            var result = await _tournamentService.GetGhost(dto);

            // Assert
            result.Should().BeNull();
        }
    }
}
