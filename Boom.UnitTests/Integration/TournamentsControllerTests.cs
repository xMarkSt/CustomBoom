using System.Linq;
using Boom.Api;
using Boom.Business.Services;
using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Boom.UnitTests.Integration;

public class TournamentsControllerTests
{
    [Test]
    public async Task Schedule_ReturnsSecretKey_ForPlainRequests()
    {
        var playerUuid = Guid.NewGuid();
        var player = CreatePlayer(playerUuid, "plain-secret");

        await using var factory = new CustomWebApplicationFactory(player, decryptedPayload: null);
        using var client = factory.CreateClient();

        var formContent = new FormUrlEncodedContent(new Dictionary<string, string?>
        {
            ["user_uuid"] = playerUuid.ToString(),
            ["badge"] = "3",
            ["nickname"] = "Plain"
        });

        var response = await client.PostAsync("/Tournaments/Schedule", formContent);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync();
        payload.Should().Contain("plain-secret");

        factory.PlayerService.LastDto.Should().NotBeNull();
        factory.PlayerService.LastDto!.badge.Should().Be(3);
        factory.PlayerService.LastDto!.nickname.Should().Be("Plain");
        factory.PlayerService.LastDto!.user_uuid.Should().Be(playerUuid);
    }

    [Test]
    public async Task Schedule_UsesDecryptedPayload_ForEncryptedRequests()
    {
        var playerUuid = Guid.NewGuid();
        var player = CreatePlayer(playerUuid, "encrypted-secret");
        var decryptedBody = $"user_uuid={Uri.EscapeDataString(playerUuid.ToString())}&badge=7&nickname=Encrypted";

        await using var factory = new CustomWebApplicationFactory(player, decryptedBody);
        factory.EncryptionService.ExpectedEncryptedPayload = "cipher";
        factory.EncryptionService.ExpectedIv = "vector";

        using var client = factory.CreateClient();

        var encryptedRequest = new FormUrlEncodedContent(new Dictionary<string, string?>
        {
            ["_p"] = "cipher",
            ["_s"] = "vector",
            ["_u"] = playerUuid.ToString(),
            ["_ct"] = "application/x-www-form-urlencoded"
        });

        var response = await client.PostAsync("/Tournaments/Schedule", encryptedRequest);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync();
        payload.Should().NotContain("encrypted-secret");

        factory.PlayerService.LastDto.Should().NotBeNull();
        factory.PlayerService.LastDto!.badge.Should().Be(7);
        factory.PlayerService.LastDto!.nickname.Should().Be("Encrypted");
        factory.PlayerService.LastDto!.user_uuid.Should().Be(playerUuid);
        factory.EncryptionService.DecryptCalls.Should().Be(1);
    }

    private static Player CreatePlayer(Guid uuid, string secret)
    {
        return new Player
        {
            Uuid = uuid,
            SecretKey = secret,
            Notification = "",
            CountryCode = "US",
            Timezone = "UTC",
            HeroStyle = "Default",
            EngineStyle = "Default",
            WheelStyle = "Default",
            Badge = 0,
            MaxGroupIdUnlocked = "1"
        };
    }

    private sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public CustomWebApplicationFactory(Player player, string? decryptedPayload)
        {
            Repository = new FakeRepository(player);
            PlayerService = new FakePlayerService(player);
            TournamentService = new FakeTournamentService();
            EncryptionService = new FakeEncryptionService { DecryptedPayload = decryptedPayload };
        }

        public FakeRepository Repository { get; }
        public FakePlayerService PlayerService { get; }
        public FakeTournamentService TournamentService { get; }
        public FakeEncryptionService EncryptionService { get; }

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(BoomDbContext));
                services.RemoveAll(typeof(DbContextOptions<BoomDbContext>));
                services.RemoveAll<IRepository>();
                services.RemoveAll<IPlayerService>();
                services.RemoveAll<IEncryptionService>();
                services.RemoveAll<ITournamentService>();

                services.AddSingleton<IRepository>(Repository);
                services.AddSingleton<IPlayerService>(PlayerService);
                services.AddSingleton<IEncryptionService>(EncryptionService);
                services.AddSingleton<ITournamentService>(TournamentService);
            });
        }
    }

    private sealed class FakeRepository : IRepository
    {
        private readonly Player _player;

        public FakeRepository(Player player)
        {
            _player = player;
        }

        public Task<T> CreateAsync<T>(T entity) where T : class, IEntity => Task.FromResult(entity);
        public Task<IEnumerable<T>> CreateAsync<T>(IEnumerable<T> entity) where T : class, IEntity => Task.FromResult(entity);
        public Task<bool> RemoveAsync<TEntity>(TEntity entity) where TEntity : class, IEntity => Task.FromResult(true);
        public Task<bool> RemoveRangeAsync<TEntity>(IEnumerable<TEntity> entity) where TEntity : class, IEntity => Task.FromResult(true);
        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity
        {
            if (typeof(TEntity) == typeof(Player))
            {
                return (new[] { (TEntity)(object)_player }).AsQueryable();
            }

            return Array.Empty<TEntity>().AsQueryable();
        }

        public TEntity? GetById<TEntity>(int id) where TEntity : class, IEntity => null;
        public TEntity? GetById<TEntity>(long id) where TEntity : class, IEntity => null;
        public Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity => Task.FromResult(entity);
        public void ClearChanges()
        {
        }
    }

    private sealed class FakePlayerService : IPlayerService
    {
        private readonly Player _player;
        public GetScheduleDto? LastDto { get; private set; }

        public FakePlayerService(Player player)
        {
            _player = player;
        }

        public Task<Player> UpdatePlayer(GetScheduleDto dto)
        {
            LastDto = dto;
            return Task.FromResult(_player);
        }
    }

    private sealed class FakeTournamentService : ITournamentService
    {
        private readonly ScheduleDto _schedule;

        public FakeTournamentService()
        {
            _schedule = new ScheduleDto
            {
                Schedule = new List<TournamentGroupDto>
                {
                    new()
                    {
                        Uuid = Guid.NewGuid(),
                        LevelId = 1,
                        Level = new LevelTargetDto
                        {
                            ThemeName = "Theme",
                            LevelName = "Level",
                            LevelId = "Level1",
                            Version = 1,
                            Target = "Target",
                            Online = true,
                            Url = "http://example.com",
                            BgName = "bg"
                        },
                        NoSuper = 0,
                        SecondsToEnd = 100,
                        SecondsToStart = 0
                    }
                }
            };
        }

        public Task<ScheduleDto> GetSchedule() => Task.FromResult(_schedule);
    }

    private sealed class FakeEncryptionService : IEncryptionService
    {
        public string? ExpectedEncryptedPayload { get; set; }
        public string? ExpectedIv { get; set; }
        public string? DecryptedPayload { get; set; }
        public int DecryptCalls { get; private set; }

        public string Encrypt(string payload, string key) => payload;

        public string Decrypt(string encryptedPayload, string key, string ivBase64)
        {
            DecryptCalls++;
            if (ExpectedEncryptedPayload != null && !string.Equals(encryptedPayload, ExpectedEncryptedPayload, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unexpected encrypted payload");
            }

            if (ExpectedIv != null && !string.Equals(ivBase64, ExpectedIv, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unexpected IV");
            }

            return DecryptedPayload ?? string.Empty;
        }

        public string GenerateSecretKey() => "generated";
        public string GetDecryptionKey(string secretKey) => "derived";
    }
}
