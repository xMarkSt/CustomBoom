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
                    context.Mapper.Map<LevelTarget, LevelTargetDto>(src.LevelTarget)))
            .ForMember(dest => dest.SecondsToEnd, opt => opt.MapFrom(src => (src.EndsAt - DateTime.Now).TotalSeconds))
            .ForMember(dest => dest.SecondsToStart,
                opt => opt.MapFrom(src => (src.StartsAt - DateTime.Now).TotalSeconds));


        CreateMap<LevelTarget, LevelTargetDto>() // todo: see php version
            .ForMember(dest => dest.ThemeName, opt => opt.MapFrom(src => src.Level.Theme.Name))
            .ForMember(dest => dest.LevelName, opt => opt.MapFrom(src => src.Level.DisplayName))
            .ForMember(dest => dest.LevelId,
                opt => opt.MapFrom(src => $"{src.Level.LevelId}:{src.Order}")) // Todo: When online, don't do this?
            .ForMember(dest => dest.Version,
                opt => opt.MapFrom(src =>
                    src.Level.Online
                        ? src.Id
                        : src.Level
                            .Version)) // Use the level target id here to ensure an entry is create in the player's ZLEVEL database for each unique goal
            .ForMember(dest => dest.Target,
                opt => opt.MapFrom(src =>
                    src.Level.Online
                        ? GetOnlineTarget()
                        : string.Empty)) //todo: when online json encode! See php. Otherwise empty.
            .ForMember(dest => dest.Online, opt => opt.MapFrom(src => src.Level.Online))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Level.Online ? GetOnlineUrl() : string.Empty))
            .ForMember(dest => dest.BgName,
                opt => opt.MapFrom(src => src.Level.Background.BgName)); // todo not working?
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