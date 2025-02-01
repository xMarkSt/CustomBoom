using AutoMapper;
using Boom.Common.DTOs;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;

namespace Boom.Business.MappingProfiles;

public class TournamentGroupProfile : Profile
{
    public TournamentGroupProfile()
    {
        CreateMap<TournamentGroup, TournamentGroupDto>()
            .ForMember(dest => dest.LevelId, 
                opt => opt.MapFrom(src => src.LevelTarget.LevelId))
            .ForMember(dest => dest.Level, 
                opt => opt.MapFrom((src, _, _, context) =>
                    context.Mapper.Map<LevelTarget, LevelTargetDto>(src.LevelTarget)))
            .ForMember(dest => dest.SecondsToEnd, 
                opt => opt.MapFrom(src => (src.EndsAt - DateTime.Now).TotalSeconds))
            .ForMember(dest => dest.SecondsToStart, 
                opt => opt.MapFrom(src => (src.StartsAt - DateTime.Now).TotalSeconds));

        // todo: see php version
        CreateMap<LevelTarget, LevelTargetDto>()
            .ForMember(dest => dest.ThemeName, 
                opt => opt.MapFrom(src => src.Level.Theme.Name))
            .ForMember(dest => dest.LevelName, 
                opt => opt.MapFrom(src => src.Level.DisplayName))
            .ForMember(dest => dest.LevelId, 
                // Todo: When online, don't do this?
                opt => opt.MapFrom(src => $"{src.Level.LevelId}:{src.Order}"))
            .ForMember(dest => dest.Version, 
                opt => opt.MapFrom(src =>
                    // Use the level target id here to ensure an entry is created in the player's ZLEVEL database for each unique goal
                    src.Level.Online ? src.Id : src.Level.Version))
            .ForMember(dest => dest.Target, 
                opt => opt.MapFrom(src =>
                    // todo: when online json encode! See php. Otherwise empty.
                    src.Level.Online ? GetOnlineTarget() : string.Empty)) 
            .ForMember(dest => dest.Online, 
                opt => opt.MapFrom(src => src.Level.Online))
            .ForMember(dest => dest.Url, 
                opt => opt.MapFrom(src => src.Level.Online ? GetOnlineUrl() : string.Empty))
            // todo not working?
            .ForMember(dest => dest.BgName, 
                opt => opt.MapFrom(src => src.Level.Background.BgName));
    }

    private string GetOnlineTarget()
    {
        throw new NotImplementedException();
    }

    private string GetOnlineUrl()
    {
        throw new NotImplementedException();
    }
}