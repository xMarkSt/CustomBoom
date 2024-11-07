using Boom.Business.Services;
using Boom.Common.DTOs;
using Boom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Boom.UnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        // var options = new DbContextOptionsBuilder<BoomDbContext>()
            // .UseInMemoryDatabase(databaseName: "Boom")
            // .Options;

        // Insert seed data into the database using one instance of the context
        // using (var context = new BoomDbContext(options))
        // {
            // context.Movies.Add(new Movie {Id = 1, Title = "Movie 1", YearOfRelease = 2018, Genre = "Action"});
            // context.Movies.Add(new Movie {Id = 2, Title = "Movie 2", YearOfRelease = 2018, Genre = "Action"});
            // context.Movies.Add(new Movie {Id = 3, Title = "Movie 3", YearOfRelease = 2019, Genre = "Action"});
            // context.SaveChanges();
        // }
    }

    public class UnitTest1
    {
        [Test]
        public void TestScheduleDto()
        {
            var options = new DbContextOptionsBuilder<BoomDbContext>()
                .UseInMemoryDatabase(databaseName: "Boom")
                .Options;
            
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
                NoSuper = false,
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
                NoSuper = true,
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
            
            // Use a clean instance of the context to run the test
            using (var context = new BoomDbContext(options))
            {
                var service = new TournamentService(context);
                var res = service.SerializeToNSDictionary(scheduleDto);
                Console.WriteLine(res.ToXmlPropertyList());
            }
        }
    }
}