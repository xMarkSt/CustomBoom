using AutoMapper;
using Boom.Common.DTOs.Request;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data.Entities;
using JoinTournamentRequestDto = Boom.Common.DTOs.Request.JoinTournamentDto;
using ReloadTournamentRequestDto = Boom.Common.DTOs.Request.ReloadTournamentDto;

namespace Boom.Business.MappingProfiles;

public class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        CreateMap<GetScheduleDto, Player>()
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.UserUuid));

        CreateMap<JoinTournamentRequestDto, Player>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.Ignore())
            .ForMember(dest => dest.SecretKey, opt => opt.Ignore())
            .ForMember(dest => dest.Badge,
                opt => { opt.Condition(src => src.Badge != null); opt.MapFrom(src => src.Badge!.Value); })
            .ForMember(dest => dest.CountryCode,
                opt => { opt.Condition(src => src.CountryCode != null); opt.MapFrom(src => src.CountryCode!); })
            .ForMember(dest => dest.Fullname,
                opt => { opt.Condition(src => src.FullName != null); opt.MapFrom(src => src.FullName); })
            .ForMember(dest => dest.MaxGroupIdUnlocked,
                opt => { opt.Condition(src => src.MaxGroupIdUnlocked != null); opt.MapFrom(src => src.MaxGroupIdUnlocked!); })
            .ForMember(dest => dest.Notification,
                opt => { opt.Condition(src => src.Notification != null); opt.MapFrom(src => src.Notification!); })
            .ForMember(dest => dest.Timezone,
                opt => { opt.Condition(src => src.Timezone != null); opt.MapFrom(src => src.Timezone!); })
            .ForMember(dest => dest.TimezoneSecondsOffset,
                opt => { opt.Condition(src => src.TimezoneSecondsOffset != null); opt.MapFrom(src => src.TimezoneSecondsOffset!.Value); })
            .ForMember(dest => dest.TotalDistance,
                opt => { opt.Condition(src => src.TotalDistance != null); opt.MapFrom(src => src.TotalDistance!.Value); })
            .ForMember(dest => dest.TotalEarnedMedals,
                opt => { opt.Condition(src => src.TotalEarnedMedals != null); opt.MapFrom(src => src.TotalEarnedMedals!.Value); })
            .ForMember(dest => dest.TotalEarnedSuperstars,
                opt => { opt.Condition(src => src.TotalEarnedSuperstars != null); opt.MapFrom(src => src.TotalEarnedSuperstars!.Value); })
            ;

        CreateMap<ReloadTournamentRequestDto, Player>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.Ignore())
            .ForMember(dest => dest.SecretKey, opt => opt.Ignore())
            .ForMember(dest => dest.Badge,
                opt => { opt.Condition(src => src.Badge != null); opt.MapFrom(src => src.Badge!.Value); })
            .ForMember(dest => dest.CountryCode,
                opt => { opt.Condition(src => src.CountryCode != null); opt.MapFrom(src => src.CountryCode!); })
            .ForMember(dest => dest.Fullname,
                opt => { opt.Condition(src => src.FullName != null); opt.MapFrom(src => src.FullName); })
            .ForMember(dest => dest.MaxGroupIdUnlocked,
                opt => { opt.Condition(src => src.MaxGroupIdUnlocked != null); opt.MapFrom(src => src.MaxGroupIdUnlocked!); })
            .ForMember(dest => dest.Notification,
                opt => { opt.Condition(src => src.Notification != null); opt.MapFrom(src => src.Notification!); })
            .ForMember(dest => dest.Timezone,
                opt => { opt.Condition(src => src.Timezone != null); opt.MapFrom(src => src.Timezone!); })
            .ForMember(dest => dest.TimezoneSecondsOffset,
                opt => { opt.Condition(src => src.TimezoneSecondsOffset != null); opt.MapFrom(src => src.TimezoneSecondsOffset!.Value); })
            .ForMember(dest => dest.TotalDistance,
                opt => { opt.Condition(src => src.TotalDistance != null); opt.MapFrom(src => src.TotalDistance!.Value); })
            .ForMember(dest => dest.TotalEarnedMedals,
                opt => { opt.Condition(src => src.TotalEarnedMedals != null); opt.MapFrom(src => src.TotalEarnedMedals!.Value); })
            .ForMember(dest => dest.TotalEarnedSuperstars,
                opt => { opt.Condition(src => src.TotalEarnedSuperstars != null); opt.MapFrom(src => src.TotalEarnedSuperstars!.Value); })
            ;

        CreateMap<Player, PlayerDto>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => (int)src.Id))
            .ForMember(dest => dest.FacebookId,
                opt => opt.MapFrom(src => src.FacebookId != null ? src.FacebookId.ToString() : null))
            .ForMember(dest => dest.TwitterId,
                opt => opt.MapFrom(src => src.TwitterId != null ? src.TwitterId.ToString() : null))
            .ForMember(dest => dest.LastLoginAt,
                opt => opt.MapFrom(src => src.LastLoginAt != null ? src.LastLoginAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt != null ? src.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt != null ? src.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null))
            .ForMember(dest => dest.Rev,
                opt => opt.MapFrom(src => src.Rev ?? 0))
            .ForMember(dest => dest.TotalHiddenPilesFound,
                opt => opt.MapFrom(src => src.TotalHiddenPilesFound ?? 0))
            .ForMember(dest => dest.TournamentsAggregatedRank,
                opt => opt.MapFrom(src => src.TournamentsAggregatedRank))
            .ForMember(dest => dest.Profile,
                opt => opt.MapFrom((src, _, _, context) =>
                    context.Mapper.Map<PlayerDto.PlayerProfileDto>(src)));

        CreateMap<Player, PlayerDto.PlayerProfileDto>()
            .ForMember(dest => dest.ChallengeStats,
                opt => opt.MapFrom((src, _, _, context) =>
                    context.Mapper.Map<PlayerDto.ChallengeStatsDto>(src)))
            .ForMember(dest => dest.TournamentStats,
                opt => opt.MapFrom((src, _, _, context) =>
                    context.Mapper.Map<PlayerDto.TournamentStatsDto>(src)));

        CreateMap<Player, PlayerDto.ChallengeStatsDto>()
            .ForMember(dest => dest.Won,
                opt => opt.MapFrom(src => src.VsWon))
            .ForMember(dest => dest.Played,
                opt => opt.MapFrom(src => src.VsPlayed))
            .ForMember(dest => dest.Lost,
                opt => opt.MapFrom(src => src.VsPlayed - src.VsWon));

        CreateMap<Player, PlayerDto.TournamentStatsDto>()
            .ForMember(dest => dest.Won,
                opt => opt.MapFrom(src => src.WcWon))
            .ForMember(dest => dest.Played,
                opt => opt.MapFrom(src => src.WcPlayed))
            .ForMember(dest => dest.AveragePos,
                opt => opt.Ignore());
    }
}