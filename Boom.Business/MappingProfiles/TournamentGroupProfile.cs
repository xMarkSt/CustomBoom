using AutoMapper;
using Boom.Common.DTOs;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.MappingProfiles;

public class TournamentGroupProfile : Profile
{
    public TournamentGroupProfile()
    {
        CreateMap<TournamentGroup, TournamentGroupDto>();
    }
}