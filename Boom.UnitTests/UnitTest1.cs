using AutoMapper;
using Boom.Business.MappingProfiles;
using Boom.Business.Services;
using Boom.Common.DTOs;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;

namespace Boom.UnitTests;

public class UnitTest1
{
    [SetUp]
    public void Setup()
    {
        
    }

    private static Level TestLevel => new()
    {
        Id = 1,
        DisplayName = "Test Level 1",
        LevelId = "FirstLeap",
        ThemeId = 1, // Mock theme ID
        Online = false,
        Custom = false,
        FilePath = "/path/to/file1",
        Version = 1,
        BgId = 10, // Mock background ID
        CreatedAt = DateTime.UtcNow.AddDays(-5), // 5 days ago
        UpdatedAt = DateTime.UtcNow, // Now
        Theme = new Theme
        {
            Id = 1,
            Name = "Sample Theme"
        },
        Background = new Theme
        {
            Id = 10,
            Name = "Sample Background",
            BgName = "MyBackground.plist"
        }
    };

    private static IQueryable<TournamentGroup> GetTestData()
    {
        return new List<TournamentGroup>
        {
            new()
            {
                Id = 1,
                Uuid = Guid.NewGuid(),
                LevelTargetId = 1,
                NoSuper = true,
                StartsAt = DateTime.Now.AddHours(-1),
                EndsAt = DateTime.Now.AddHours(1),
                LevelTarget = new LevelTarget
                {
                    Id = 1,
                    LevelId = 1,
                    TargetId = 1,
                    TargetAmount = 10,
                    Order = 1,
                    Level = TestLevel,
                    Target = new Target
                    {
                        Id = 1,
                        Type = "Collect all Coins"
                    },
                },
            }
        }.AsQueryable();
    }

    [Test]
    public async Task TestMapping()
    {
        // Arrange
        var mockRepo = new Mock<IRepository>();
        mockRepo
            .Setup(r => r.GetAll<TournamentGroup>())
            .Returns(GetTestData().BuildMock);
        
        var profile = new TournamentGroupProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
        var mapper = new Mapper(configuration);

        // Act
        var service = new TournamentService(mockRepo.Object, mapper);
        var res = await service.GetScheduled();
        Console.WriteLine(service.SerializeToNSDictionary(res).ToXmlPropertyList());
    }

    [Test]
    public void TestSerialization()
    {
        // Create dummy data for TournamentGroupDto
        var tournamentGroup1 = new TournamentGroupDto
        {
            Uuid = Guid.NewGuid(),
            LevelId = 1,
            Level = new LevelDto
            {
                ThemeName = "MyTheme",
                LevelName = "Dummy Level",
                LevelId = "D1",
                Version = 1,
                Target = "Dummy Target",
                Online = true,
                Url = "https://dummy.url",
                BgName = "DummyBg"
            },
            NoSuper = 0,
            SecondsToEnd = 60,
            SecondsToStart = 30
        };

        var tournamentGroup2 = new TournamentGroupDto
        {
            Uuid = Guid.NewGuid(),
            LevelId = 2,
            Level = new LevelDto
            {
                ThemeName = "Another Theme",
                LevelName = "Another Level",
                LevelId = "A2",
                Version = 2,
                Target = "",
                Online = false,
                Url = "",
                BgName = "AnotherBg"
            },
            NoSuper = 1,
            SecondsToEnd = 120,
            SecondsToStart = 60
        };

        // Create ScheduleDto with dummy data
        var scheduleDto = new ScheduleDto
        {
            Schedule = new List<TournamentGroupDto>
            {
                tournamentGroup1,
                tournamentGroup2
            }
        };

        var service = new TournamentService(Mock.Of<IRepository>(), Mock.Of<IMapper>());
        var res = service.SerializeToNSDictionary(scheduleDto);
        Console.WriteLine(res.ToXmlPropertyList());
    }
}