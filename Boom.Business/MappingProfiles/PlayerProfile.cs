using AutoMapper;
using Boom.Common.DTOs.Request;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.MappingProfiles;

public class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        CreateMap<GetScheduleDto, Player>();
    }
}