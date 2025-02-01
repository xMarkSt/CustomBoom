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
}