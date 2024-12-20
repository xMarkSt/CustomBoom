using Boom.Business.Services;
using Boom.Common.DTOs;
using Claunia.PropertyList;
using FluentAssertions;

namespace Boom.UnitTests;

public class PlistSerializationServiceTests
{
    private static TournamentGroupDto testSchedule = new()
    {
        Uuid = new Guid("800c4278-633d-45ac-9d97-4aad5950be6b"),
        LevelId = 61,
        Level = new LevelTargetDto
        {
            ThemeName = "Frozen Hills",
            LevelName = "Waterslide",
            LevelId = "Waterslide:0",
            Version = 1,
            Target = "", // Only filled for online levels
            Online = false,
            Url = "", // Only filled for online levels
            BgName = "NorthBG.plist"
        },
        NoSuper = 0,
        SecondsToEnd = 69164,
        SecondsToStart = -17236
    };
    
    [Test]
    public void SerializeToNSDictionary_Equals_TestSchedule()
    {
        // Create ScheduleDto with dummy data
        var scheduleDto = new ScheduleDto
        {
            Schedule = [testSchedule]
        };

        var service = new PlistSerializationService();
        var res = service.SerializeToNSDictionary(scheduleDto);
        var actualDict = (NSDictionary)PropertyListParser.Parse("Responses\\schedule.plist");
        
        res.ToXmlPropertyList().Should().BeEquivalentTo(actualDict.ToXmlPropertyList());
    }
}