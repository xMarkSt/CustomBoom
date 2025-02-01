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
    public class PlayerServiceTests
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IEncryptionService> _mockEncryptionService;
        private Mock<IMapper> _mockMapper;
        private PlayerService _playerService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _mockEncryptionService = new Mock<IEncryptionService>();
            _mockMapper = new Mock<IMapper>();
            _playerService = new PlayerService(_mockRepository.Object, _mockEncryptionService.Object, _mockMapper.Object);
        }

        [Test]
        public async Task UpdatePlayer_PlayerDoesNotExist_CreatesNewPlayer()
        {
            // Arrange
            var dto = new GetScheduleDto { user_uuid = Guid.NewGuid() };
            var player = new Player();
            _mockRepository.Setup(r => r.GetAll<Player>()).Returns(new List<Player>().AsQueryable().BuildMock());
            _mockMapper.Setup(m => m.Map<Player>(dto)).Returns(player);
            _mockEncryptionService.Setup(e => e.GenerateSecretKey()).Returns("secretkey");

            // Act
            await _playerService.UpdatePlayer(dto);

            // Assert
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Player>()), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Player>()), Times.Never);
            player.SecretKey.Should().Be("secretkey");
        }

        [Test]
        public async Task UpdatePlayer_PlayerExists_UpdatesExistingPlayer()
        {
            // Arrange
            var dto = new GetScheduleDto { user_uuid = Guid.NewGuid() };
            var existingPlayer = new Player { Uuid = dto.user_uuid };
            var players = new List<Player> { existingPlayer }.AsQueryable().BuildMock();
            _mockRepository.Setup(r => r.GetAll<Player>()).Returns(players);

            // Act
            await _playerService.UpdatePlayer(dto);

            // Assert
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Player>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateAsync(existingPlayer), Times.Once);
        }
    }
}