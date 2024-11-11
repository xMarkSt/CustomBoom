using AutoMapper;
using Boom.Common.DTOs;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.MappingProfiles;

public class TournamentGroupProfile : Profile
{
    public TournamentGroupProfile()
    {
        CreateMap<TournamentGroup, TournamentGroupDto>()
            .ForMember(dest => dest.LevelId, opt => opt.MapFrom(src => src.LevelTarget.LevelId))
            .ForMember(dest => dest.Level,
                opt => opt.MapFrom((src, _, _, context) =>
                    context.Mapper.Map<LevelTarget, LevelDto>(src.LevelTarget)));
        CreateMap<LevelTarget, LevelDto>() // todo see php version
            // .ForMember(dest => dest.LevelId, opt => opt.MapFrom(src => $"{src.LevelId}:{src.Order}"))
            // .ForMember(dest => dest.Target, opt => opt.MapFrom(src => $"{src.LevelId}:{src.Order}")) 
            ;
    }
}