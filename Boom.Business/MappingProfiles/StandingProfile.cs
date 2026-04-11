using AutoMapper;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.MappingProfiles;

public class StandingProfile : Profile
{
    public StandingProfile()
    {
        CreateMap<Standing, StandingDto>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => (int)src.Id))
            .ForMember(dest => dest.TournamentId,
                opt => opt.MapFrom(src => (int)src.TournamentId))
            .ForMember(dest => dest.UserId,
                opt => opt.MapFrom(src => (int)src.UserId))
            .ForMember(dest => dest.GhostId,
                opt => opt.MapFrom(src => (int)src.GhostId))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt != null ? src.CreatedAt.Value.ToString("o") : null))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt != null ? src.UpdatedAt.Value.ToString("o") : null))
            .ForMember(dest => dest.BoomUser,
                opt => opt.MapFrom((src, _, _, context) =>
                    context.Mapper.Map<PlayerDto>(src.Player)))
            .ForMember(dest => dest.Rank,
                opt => opt.Ignore())
            .ForMember(dest => dest.IsSelf,
                opt => opt.Ignore());
    }
}
