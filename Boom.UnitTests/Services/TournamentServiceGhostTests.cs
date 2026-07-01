using System.IO.Compression;
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

        private static byte[] Gzip(byte[] data)
        {
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        [Test]
        public async Task GetGhost_OpponentStandingExists_ReturnsDecompressedGhost()
        {
            // Arrange: ghosts are stored gzip-compressed (as uploaded by the client)
            var tournamentUuid = Guid.NewGuid();
            var opponentUuid = Guid.NewGuid();
            var replay = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var storedBytes = Gzip(replay);

            var standing = new Standing
            {
                Tournament = new Tournament { Uuid = tournamentUuid },
                Player = new Player { Uuid = opponentUuid },
                Ghost = new Ghost { Data = storedBytes }
            };
            SetupStandings(new[] { standing });

            var dto = new GhostTournamentDto { TournamentUuid = tournamentUuid, OpponentUuid = opponentUuid };

            // Act
            var result = await _tournamentService.GetGhost(dto);

            // Assert: the decompressed replay is returned, matching PHP Ghost::getDataAttribute (gzdecode)
            result.Should().Equal(replay);
        }

        [Test]
        public async Task GetGhost_DataNotGzip_ReturnsAsIs()
        {
            // Legacy/uncompressed rows must not crash the download.
            var tournamentUuid = Guid.NewGuid();
            var opponentUuid = Guid.NewGuid();
            var rawBytes = new byte[] { 1, 2, 3, 4, 5 }; // not gzip-framed

            var standing = new Standing
            {
                Tournament = new Tournament { Uuid = tournamentUuid },
                Player = new Player { Uuid = opponentUuid },
                Ghost = new Ghost { Data = rawBytes }
            };
            SetupStandings(new[] { standing });

            var dto = new GhostTournamentDto { TournamentUuid = tournamentUuid, OpponentUuid = opponentUuid };

            // Act
            var result = await _tournamentService.GetGhost(dto);

            // Assert
            result.Should().Equal(rawBytes);
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
