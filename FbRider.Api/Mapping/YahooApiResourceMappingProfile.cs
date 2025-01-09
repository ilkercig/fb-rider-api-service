using AutoMapper;
using FbRider.Api.Domain.Models;
using FbRider.Api.Utils;
using FbRider.Api.YahooApi;
using ManagerResource = FbRider.Api.YahooApi.Manager;
using Manager = FbRider.Api.Domain.Models.Manager;
using RosterPosition = FbRider.Api.Domain.Models.RosterPosition;
using RosterPositionResource = FbRider.Api.YahooApi.RosterPosition;

namespace FbRider.Api.Mapping;

public class YahooApiResourceMappingProfile : Profile
{
    public YahooApiResourceMappingProfile()
    {
        CreateMap<Stat, StatCategory>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StatId))
            .ForMember(dest => dest.Abbreviation, opt => opt.MapFrom(src => src.Abbr))
            .ForMember(dest => dest.IsOnlyDisplayStat, opt => opt.MapFrom(src => src.IsOnlyDisplayStat == "1"))
            .ForAllMembers(opt =>
            {
                opt.Condition((src, dest, srcMember) => srcMember != null); // Ignore null values
            });

        CreateMap<RosterPositionResource, RosterPosition>().ForMember(dest => dest.IsStartingPosition,
            opt => opt.MapFrom(src => src.IsStartingPosition == "1"));
        CreateMap<Settings, LeagueSettings>().ForMember(dest => dest.StatCategories,
                opt => opt.MapFrom(src => src.StatCategories.Stats))
            .ForMember(dest => dest.RosterPositions, opt => opt.MapFrom(src => src.RosterPositions));

        CreateMap<Player, FantasyPlayer>().ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.PlayerKey))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PlayerId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name.Full))
            .ForMember(dest => dest.DisplayPositions,
                opt => opt.MapFrom(src => src.DisplayPosition.Split(new[] { ',' })))
            .ForMember(dest => dest.IsUndroppable, opt => opt.MapFrom(src => src.IsUndroppable == "1"))
            .ForMember(dest => dest.SelectedPosition,
                opt => opt.MapFrom(src => src.SelectedPosition == null ? null : src.SelectedPosition.Position))
            .ForAllMembers(opt =>
            {
                opt.Condition((src, dest, srcMember) => srcMember != null); // Ignore null values
            });

        CreateMap<Roster, FantasyTeamRoster>()
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players));

        CreateMap<ManagerResource, Manager>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nickname))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ManagerId))
            .ForMember(dest => dest.IsCommissioner, opt => opt.MapFrom(src => src.IsCommissioner == "1"))
            .ForMember(dest => dest.IsCurrentLogin, opt => opt.MapFrom(src => src.IsCurrentLogin == "1"));

        CreateMap<Team, FantasyTeam>().ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.TeamKey))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TeamId))
            .ForMember(dest => dest.TeamLogo,
                opt => opt.MapFrom(src => src.TeamLogos == null ? null : src.TeamLogos.First().Url))
            .ForMember(dest => dest.Managers, opt => opt.MapFrom(src => src.Managers))
            .ForMember(dest => dest.Roster, opt => opt.MapFrom(src => src.Roster));

        CreateMap<League, FantasyLeague>().ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.LeagueKey))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LeagueId))
            .ForMember(dest => dest.CurrentWeek, opt => opt.MapFrom(src => Convert.ToInt32(src.CurrentWeek)))
            .ForMember(dest => dest.ScoringType,
                opt => opt.MapFrom(src => EnumConvertor.GetScoringType(src.ScoringType)))
            .ForMember(dest => dest.Teams,
                opt => opt.MapFrom(src => src.Standings == null ? null : src.Standings.Teams));

        CreateMap<Game, FantasySeason>().ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.GameKey))
            .ForMember(dest => dest.IsSeasonOver, opt => opt.MapFrom(src => src.IsGameOver == "1"))
            .ForMember(dest => dest.Leagues, opt => opt.MapFrom(src => src.Leagues));

        CreateMap<User, FantasyUser>().ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Guid))
            .ForMember(dest => dest.Seasons, opt => opt.MapFrom(src => src.Games));
    }
}