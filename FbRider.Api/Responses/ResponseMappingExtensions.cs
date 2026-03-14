using FbRider.Domain.Models;

namespace FbRider.Api.Responses;

public static class ResponseMappingExtensions
{
    public static LeagueResponse ToResponse(this League league) => new(
        league.Key,
        league.Id,
        league.Name,
        league.LogoUrl,
        league.ScoringType.ToString(),
        league.CurrentWeek,
        league.StartWeek
    );

    public static SeasonResponse ToResponse(this Season season) => new(
        season.Key,
        season.SeasonYear,
        season.IsSeasonOver,
        season.Leagues.Select(l => l.ToResponse())
    );

    public static TeamResponse ToResponse(this Team team) => new(
        team.Key,
        team.Id,
        team.Name,
        team.TeamLogo,
        team.Managers.Select(m => m.ToResponse()),
        team.Roster?.ToResponse()
    );

    public static ManagerResponse ToResponse(this Manager manager) => new(
        manager.Id,
        manager.Name,
        manager.IsCurrentLogin,
        manager.IsCommissioner
    );

    public static TeamRosterResponse ToResponse(this TeamRoster roster) => new(
        roster.Players.Select(p => p.ToResponse())
    );

    public static PlayerResponse ToResponse(this Player player) => new(
        player.Key,
        player.Id,
        player.FullName,
        player.ImageUrl,
        player.EligiblePositions,
        player.DisplayPositions,
        player.SelectedPosition,
        player.IsUndroppable,
        player.Status
    );

    public static LeagueSettingsResponse ToResponse(this LeagueSettings settings) => new(
        settings.StatCategories.Select(s => s.ToResponse()),
        settings.RosterPositions.Select(r => r.ToResponse())
    );

    public static StatCategoryResponse ToResponse(this StatCategory stat) => new(
        stat.Id,
        stat.Name,
        stat.DisplayName,
        stat.Abbreviation,
        stat.SortOrder,
        stat.IsOnlyDisplayStat
    );

    public static RosterPositionResponse ToResponse(this RosterPosition pos) => new(
        pos.Position,
        pos.Count,
        pos.IsStartingPosition
    );
}
