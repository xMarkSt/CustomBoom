using Boom.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Boom.UnitTests.Services;

/// <summary>
/// Shared entity builders for TournamentService test fixtures (Join/Update/Ghost).
/// </summary>
internal static class TournamentTestFixtures
{
    public static Player Player(long id) => new() { Id = id, Uuid = Guid.NewGuid() };

    public static Standing Standing(long id, long userId, int time, long ghostId) => new()
    {
        Id = id,
        UserId = userId,
        Time = time,
        GhostId = ghostId,
        HeroStyle = "old",
        WheelStyle = "old",
        EngineStyle = "old",
        Player = new Player { Id = userId, Uuid = Guid.NewGuid(), Nickname = $"P{userId}" }
    };

    public static Tournament Tournament(Guid uuid, params Standing[] standings) => new()
    {
        Id = 5,
        Uuid = uuid,
        Standings = standings.ToList()
    };

    public static Tournament Tournament(params Standing[] standings) => Tournament(Guid.NewGuid(), standings);

    public static IFormFile GhostFile(byte[] data)
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream target, CancellationToken _) =>
            {
                target.Write(data, 0, data.Length);
                return Task.CompletedTask;
            });
        return mock.Object;
    }
}
