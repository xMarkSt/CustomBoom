using Boom.Api.Middleware;
using Boom.Business.Services;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Boom.UnitTests.Middleware;

public class RequestDecryptionMiddlewareTests
{
    [Test]
    public async Task InvokeAsync_PopulatesFormWithDecryptedValues()
    {
        var playerUuid = Guid.NewGuid();
        var encryptedPayload = "encrypted";
        var decryptedPayload = $"user_uuid={Uri.EscapeDataString(playerUuid.ToString())}&badge=5&nickname=Test";
        var player = new Player
        {
            Uuid = playerUuid,
            SecretKey = "secret"
        };

        var repository = new Mock<IRepository>();
        repository.Setup(r => r.GetAll<Player>()).Returns(new[] { player }.AsQueryable());

        var encryption = new Mock<IEncryptionService>();
        encryption.Setup(s => s.GetDecryptionKey(player.SecretKey!)).Returns("derived");
        encryption.Setup(s => s.Decrypt(encryptedPayload, "derived", "iv")).Returns(decryptedPayload);

        var services = new ServiceCollection();
        services.AddSingleton(repository.Object);
        services.AddSingleton(encryption.Object);
        services.AddSingleton(Mock.Of<IPlayerService>());
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var middleware = new RequestDecryptionMiddleware(_ => Task.CompletedTask, scopeFactory, NullLogger<RequestDecryptionMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.QueryString = QueryString.Create(new Dictionary<string, string?>
        {
            ["_p"] = encryptedPayload,
            ["_s"] = "iv",
            ["_u"] = playerUuid.ToString(),
            ["_ct"] = "application/x-www-form-urlencoded"
        });

        await middleware.InvokeAsync(context);

        context.Items[RequestDecryptionMiddleware.RequestEncryptedItemKey].Should().Be(true);
        context.Request.Form["user_uuid"].ToString().Should().Be(playerUuid.ToString());
        context.Request.Form["badge"].ToString().Should().Be("5");
        context.Request.Form["nickname"].ToString().Should().Be("Test");
    }

    [Test]
    public async Task InvokeAsync_PlayerMissing_DoesNotAlterRequest()
    {
        var repository = new Mock<IRepository>();
        repository.Setup(r => r.GetAll<Player>()).Returns(Array.Empty<Player>().AsQueryable());

        var encryption = new Mock<IEncryptionService>();
        encryption.Setup(s => s.GetDecryptionKey(It.IsAny<string>())).Returns("ignored");
        encryption.Setup(s => s.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("ignored");

        var services = new ServiceCollection();
        services.AddSingleton(repository.Object);
        services.AddSingleton(encryption.Object);
        services.AddSingleton(Mock.Of<IPlayerService>());
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var middleware = new RequestDecryptionMiddleware(_ => Task.CompletedTask, scopeFactory, NullLogger<RequestDecryptionMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.QueryString = QueryString.Create(new Dictionary<string, string?>
        {
            ["_p"] = "encrypted",
            ["_s"] = "iv",
            ["_u"] = Guid.NewGuid().ToString(),
            ["_ct"] = "application/x-www-form-urlencoded"
        });

        await middleware.InvokeAsync(context);

        context.Items[RequestDecryptionMiddleware.RequestEncryptedItemKey].Should().Be(false);
        context.Features.Get<IFormFeature>().Should().BeNull();
    }
}
