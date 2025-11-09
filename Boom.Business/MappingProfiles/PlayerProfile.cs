using AutoMapper;
using Boom.Common.DTOs.Request;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.MappingProfiles;

public class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        CreateMap<GetScheduleDto, Player>()
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.user_uuid));
    }
}