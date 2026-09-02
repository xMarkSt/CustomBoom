using Boom.Business.Services;
using Boom.Common.DTOs.Response;
using Claunia.PropertyList;
using FluentAssertions;

namespace Boom.UnitTests.Services;

public class PlistSerializationServiceTests
{
    [Test]
    public void SerializeToNSDictionary_Equals_TestSchedule()
    {
        // Create ScheduleDto with dummy data
        var scheduleDto = new ScheduleDto
        {
            Schedule = [TestData.TournamentGroupDto]
        };

        var service = new PlistSerializationService();
        var res = service.SerializeToNSDictionary(scheduleDto);
        var actualDict = (NSDictionary)PropertyListParser.Parse("Responses\\schedule.plist");
        
        res.ToXmlPropertyList().Should().BeEquivalentTo(actualDict.ToXmlPropertyList());
    }

    [Test]
    public void SerializeToNSDictionary_IncludesSecretKey_WhenPresent()
    {
        var service = new PlistSerializationService();
        var dto = new ScheduleDto
        {
            SecretKey = "__abc.12345678",
            Schedule = []
        };

        var plist = service.SerializeToNSDictionary(dto);

        plist.ToXmlPropertyList().Should().Contain("_sk");
    }

    [Test]
    public void SerializeToNSDictionary_ResultsResponse_KeysRootByTournamentUuid()
    {
        var service = new PlistSerializationService();
        var tournamentUuid = Guid.NewGuid();
        var dto = new TournamentResultsDto
        {
            [tournamentUuid] = new TournamentResultDto
            {
                Completed = 1,
                Rank = 2,
                Standings = [new StandingDto
                {
                    HeroStyle = "h", WheelStyle = "w", EngineStyle = "e",
                    CreatedAt = "", UpdatedAt = "",
                    BoomUser = new PlayerDto(),
                    Rank = 1
                }]
            }
        };

        var xml = service.SerializeToNSDictionary(dto).ToXmlPropertyList();

        xml.Should().Contain($"<key>{tournamentUuid}</key>");
        xml.Should().Contain("<key>completed</key>");
        xml.Should().Contain("<key>rank</key>");
        xml.Should().Contain("<key>standings</key>");
    }
    
    [Test]
    public void SerializeToNSDictionary_ArticlesAsEmptyList_SerializesToEmptyArray()
    {
        var service = new PlistSerializationService();
        var dto = new NewsFeedDto { Articles = new List<ArticleDto>(), Count = 0, New = 0 };

        var plist = (NSDictionary)service.SerializeToNSDictionary(dto);

        plist["articles"].Should().BeOfType<NSArray>();
        ((NSArray)plist["articles"]).Count.Should().Be(0);
    }

    [Test]
    public void SerializeToNSDictionary_ArticlesAsDictionary_SerializesToDictionaryKeyedByGivenKeys()
    {
        var service = new PlistSerializationService();
        var dto = new NewsFeedDto
        {
            Articles = new Dictionary<string, ArticleDto>
            {
                ["1700000000.5"] = new ArticleDto
                {
                    Id = 5,
                    Timestamp = 1700000000,
                    Title = "Title",
                    Message = "Message",
                    Link = "https://example.com",
                    LinkTitle = "Read more",
                    Popup = 1,
                },
            },
            Count = 1,
            New = 1,
        };

        var plist = (NSDictionary)service.SerializeToNSDictionary(dto);

        var articles = (NSDictionary)plist["articles"];
        articles.ContainsKey("1700000000.5").Should().BeTrue();

        var xml = plist.ToXmlPropertyList();
        xml.Should().Contain("1700000000.5");
        xml.Should().Contain("<key>title</key>").And.Contain("Title");
        xml.Should().Contain("<key>link_title</key>").And.Contain("Read more");
        xml.Should().Contain("<key>popup</key>");
    }
}
