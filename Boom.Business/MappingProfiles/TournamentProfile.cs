using AutoMapper;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.MappingProfiles;

public class TournamentProfile : Profile
{
    public TournamentProfile()
    {
        CreateMap<Tournament, TournamentDto>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => (int)src.Id))
            .ForMember(dest => dest.TournamentGroupId,
                opt => opt.MapFrom(src => (int)src.TournamentGroupId))
            .ForMember(dest => dest.Users,
                opt => opt.MapFrom(src => src.Standings.Count))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt != null ? src.CreatedAt.Value.ToString("o") : null))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt != null ? src.UpdatedAt.Value.ToString("o") : null));
    }
}
