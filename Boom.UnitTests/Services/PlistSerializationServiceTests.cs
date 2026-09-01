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
        var dto = new ResultsResponseDto
        {
            [tournamentUuid] = new TournamentResultsDto
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
}
