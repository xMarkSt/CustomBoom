using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boom.Infrastructure.Seeding;

public class SeedHelper
{
    public static async Task SeedLevels(DbContext context, CancellationToken cancellationToken)
    {
        var levels = new List<Level>
        {
            new()
            {
                Id = 1, DisplayName = "First Leap", LevelId = "FirstLeap", ThemeId = 1, Online = false,
                Custom = false, Version = 2, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 2, DisplayName = "Acrobat", LevelId = "Acrobat", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 3, DisplayName = "The staircase", LevelId = "TheStaircase", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 4, DisplayName = "Air Moves", LevelId = "AirMoves", ThemeId = 1, Online = false,
                Custom = false, Version = 3, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 5, DisplayName = "Going Down", LevelId = "GoingDown", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 6, DisplayName = "Robot", LevelId = "Robot", ThemeId = 1, Online = false, Custom = false,
                Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 7, DisplayName = "To Heaven", LevelId = "ToHeaven", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 8, DisplayName = "Blokks", LevelId = "Blokks", ThemeId = 1, Online = false, Custom = false,
                Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 9, DisplayName = "Underground", LevelId = "Underground", ThemeId = 1, Online = false,
                Custom = false, Version = 2, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 10, DisplayName = "Boost Race 1", LevelId = "BoostRace1", ThemeId = 1, Online = false,
                Custom = false, Version = 3, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 11, DisplayName = "Runner Up", LevelId = "RunnerUp", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 12, DisplayName = "Tube", LevelId = "Tube", ThemeId = 1, Online = false, Custom = false,
                Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 13, DisplayName = "Sky Castle", LevelId = "SkyCastle", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 14, DisplayName = "Here is Boom!", LevelId = "HereIsBoom", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 15, DisplayName = "Balance", LevelId = "Balance", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 16, DisplayName = "Spring Jump", LevelId = "SpringJump", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 17, DisplayName = "Big Loop", LevelId = "BigLoop", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 18, DisplayName = "Whirl", LevelId = "Whirl", ThemeId = 1, Online = false, Custom = false,
                Version = 1, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 19, DisplayName = "Rocket School", LevelId = "RocketSchool", ThemeId = 1, Online = false,
                Custom = false, Version = 2, BgId = 1, FilePath = null
            },
            new()
            {
                Id = 20, DisplayName = "Loooops", LevelId = "Loooops", ThemeId = 1, Online = false,
                Custom = false, Version = 1, BgId = 1, FilePath = null
            },
        };

        await context.Set<Level>().AddRangeAsync(levels, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}