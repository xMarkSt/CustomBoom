using System.Text.Json;
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
                opt => opt.MapFrom(src => (src.EndsAt - DateTime.UtcNow).TotalSeconds))
            .ForMember(dest => dest.SecondsToStart, 
                opt => opt.MapFrom(src => (src.StartsAt - DateTime.UtcNow).TotalSeconds));

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
                    src.Level.Online ? GetOnlineTarget(src) : string.Empty))
            .ForMember(dest => dest.Online,
                opt => opt.MapFrom(src => src.Level.Online))
            .ForMember(dest => dest.Url,
                opt => opt.MapFrom(src => src.Level.Online ? GetOnlineUrl(src.Level) : string.Empty))
            // todo not working?
            .ForMember(dest => dest.BgName, 
                opt => opt.MapFrom(src => src.Level.Background.BgName));
    }

    private static string GetOnlineTarget(LevelTarget levelTarget)
    {
        if (levelTarget.Target.Type == "Fastest time")
        {
            return string.Empty;
        }

        var target = new Dictionary<string, object>
        {
            ["Type"] = levelTarget.Target.Type
        };
        if (levelTarget.TargetAmount != null)
        {
            target["Target"] = levelTarget.TargetAmount;
        }

        return JsonSerializer.Serialize(target);
    }

    private static string GetOnlineUrl(Level level)
    {
        var baseUrl = Environment.GetEnvironmentVariable("APP_URL") ?? string.Empty;

        if (!string.IsNullOrEmpty(level.FilePath))
        {
            return $"{baseUrl}/storage/{level.FilePath.TrimStart('/')}";
        }

        return $"{baseUrl}/online-levels/{level.LevelId}.plhs";
    }
}