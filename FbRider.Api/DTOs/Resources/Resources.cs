// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(FantasyContent));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (FantasyContent)serializer.Deserialize(reader);
// }

using System.Xml.Serialization;

namespace FbRider.Api.DTOs.Resources;

[XmlRoot(ElementName = "waiver_days", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class WaiverDays
{

    [XmlElement(ElementName = "day", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<int> Day { get; set; }
}

[XmlRoot(ElementName = "roster_position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class RosterPosition
{

    [XmlElement(ElementName = "position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Position { get; set; }

    [XmlElement(ElementName = "position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string PositionType { get; set; }

    [XmlElement(ElementName = "count", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Count { get; set; }

    [XmlElement(ElementName = "is_starting_position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsStartingPosition { get; set; }
}

[XmlRoot(ElementName = "roster_positions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class RosterPositions
{

    [XmlElement(ElementName = "roster_position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<RosterPosition> RosterPosition { get; set; }
}

[XmlRoot(ElementName = "stat_position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class StatPositionType
{

    [XmlElement(ElementName = "position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string PositionType { get; set; }

    [XmlElement(ElementName = "is_only_display_stat", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsOnlyDisplayStat { get; set; }
}

[XmlRoot(ElementName = "stat_position_types", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class StatPositionTypes
{

    [XmlElement(ElementName = "stat_position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required StatPositionType StatPositionType { get; set; }
}

[XmlRoot(ElementName = "stat", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Stat
{

    [XmlElement(ElementName = "stat_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int StatId { get; set; }

    [XmlElement(ElementName = "enabled", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? Enabled { get; set; }

    [XmlElement(ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "display_name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? DisplayName { get; set; }

    [XmlElement(ElementName = "group", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? Group { get; set; }

    [XmlElement(ElementName = "abbr", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? Abbr { get; set; }

    [XmlElement(ElementName = "sort_order", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? SortOrder { get; set; }

    [XmlElement(ElementName = "position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? PositionType { get; set; }

    [XmlElement(ElementName = "stat_position_types", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public StatPositionTypes? StatPositionTypes { get; set; }

    [XmlElement(ElementName = "is_only_display_stat", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? IsOnlyDisplayStat { get; set; }

    [XmlElement(ElementName = "value", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Value { get; set; }
}

[XmlRoot(ElementName = "stats", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Stats
{

    [XmlElement(ElementName = "stat", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<Stat> Stat { get; set; }
}

[XmlRoot(ElementName = "group", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Group
{

    [XmlElement(ElementName = "group_name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string GroupName { get; set; }

    [XmlElement(ElementName = "group_display_name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string GroupDisplayName { get; set; }

    [XmlElement(ElementName = "group_abbr", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string GroupAbbr { get; set; }
}

[XmlRoot(ElementName = "groups", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Groups
{

    [XmlElement(ElementName = "group", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<Group> Group { get; set; }
}

[XmlRoot(ElementName = "stat_categories", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class StatCategories
{

    [XmlElement(ElementName = "stats", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Stats Stats { get; set; }

    [XmlElement(ElementName = "groups", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Groups Groups { get; set; }
}

[XmlRoot(ElementName = "settings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Settings
{

    [XmlElement(ElementName = "draft_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string DraftType { get; set; }

    [XmlElement(ElementName = "is_auction_draft", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsAuctionDraft { get; set; }

    [XmlElement(ElementName = "scoring_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string ScoringType { get; set; }

    [XmlElement(ElementName = "persistent_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string PersistentUrl { get; set; }

    [XmlElement(ElementName = "uses_playoff", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int UsesPlayoff { get; set; }

    [XmlElement(ElementName = "has_playoff_consolation_games", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int HasPlayoffConsolationGames { get; set; }

    [XmlElement(ElementName = "playoff_start_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int PlayoffStartWeek { get; set; }

    [XmlElement(ElementName = "uses_playoff_reseeding", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int UsesPlayoffReseeding { get; set; }

    [XmlElement(ElementName = "uses_lock_eliminated_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int UsesLockEliminatedTeams { get; set; }

    [XmlElement(ElementName = "num_playoff_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int NumPlayoffTeams { get; set; }

    [XmlElement(ElementName = "num_playoff_consolation_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int NumPlayoffConsolationTeams { get; set; }

    [XmlElement(ElementName = "has_multiweek_championship", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int HasMultiweekChampionship { get; set; }

    [XmlElement(ElementName = "waiver_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string WaiverType { get; set; }

    [XmlElement(ElementName = "waiver_rule", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string WaiverRule { get; set; }

    [XmlElement(ElementName = "waiver_days", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required WaiverDays WaiverDays { get; set; }

    [XmlElement(ElementName = "uses_faab", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int UsesFaab { get; set; }

    [XmlElement(ElementName = "draft_time", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int DraftTime { get; set; }

    [XmlElement(ElementName = "draft_pick_time", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int DraftPickTime { get; set; }

    [XmlElement(ElementName = "post_draft_players", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string PostDraftPlayers { get; set; }

    [XmlElement(ElementName = "max_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int MaxTeams { get; set; }

    [XmlElement(ElementName = "waiver_time", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int WaiverTime { get; set; }

    [XmlElement(ElementName = "trade_end_date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime TradeEndDate { get; set; }

    [XmlElement(ElementName = "trade_ratify_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string TradeRatifyType { get; set; }

    [XmlElement(ElementName = "trade_reject_time", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int TradeRejectTime { get; set; }

    [XmlElement(ElementName = "player_pool", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string PlayerPool { get; set; }

    [XmlElement(ElementName = "cant_cut_list", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string CantCutList { get; set; }

    [XmlElement(ElementName = "draft_together", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int DraftTogether { get; set; }

    [XmlElement(ElementName = "is_publicly_viewable", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsPubliclyViewable { get; set; }

    [XmlElement(ElementName = "sendbird_channel_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string SendbirdChannelUrl { get; set; }

    [XmlElement(ElementName = "roster_positions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required RosterPositions RosterPositions { get; set; }

    [XmlElement(ElementName = "stat_categories", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required StatCategories StatCategories { get; set; }

    [XmlElement(ElementName = "max_weekly_adds", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int MaxWeeklyAdds { get; set; }

    [XmlElement(ElementName = "uses_median_score", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required object UsesMedianScore { get; set; }

    [XmlElement(ElementName = "league_premium_features", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required object LeaguePremiumFeatures { get; set; }
}

[XmlRoot(ElementName = "team_logo", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class TeamLogo
{

    [XmlElement(ElementName = "size", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Size { get; set; }

    [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Url { get; set; }
}

[XmlRoot(ElementName = "team_logos", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class TeamLogos
{

    [XmlElement(ElementName = "team_logo", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required TeamLogo TeamLogo { get; set; }
}

[XmlRoot(ElementName = "roster_adds", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class RosterAdds
{

    [XmlElement(ElementName = "coverage_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string CoverageType { get; set; }

    [XmlElement(ElementName = "coverage_value", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int CoverageValue { get; set; }

    [XmlElement(ElementName = "value", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Value { get; set; }
}

[XmlRoot(ElementName = "manager", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Manager
{

    [XmlElement(ElementName = "manager_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int ManagerId { get; set; }

    [XmlElement(ElementName = "nickname", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Nickname { get; set; }

    [XmlElement(ElementName = "guid", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Guid { get; set; }

    [XmlElement(ElementName = "email", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Email { get; set; }

    [XmlElement(ElementName = "image_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string ImageUrl { get; set; }

    [XmlElement(ElementName = "felo_score", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int FeloScore { get; set; }

    [XmlElement(ElementName = "felo_tier", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string FeloTier { get; set; }

    [XmlElement(ElementName = "is_commissioner", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? IsCommissioner { get; set; }

    [XmlElement(ElementName = "is_current_login", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? IsCurrentLogin { get; set; }
}

[XmlRoot(ElementName = "managers", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Managers
{

    [XmlElement(ElementName = "manager", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Manager Manager { get; set; }
}

[XmlRoot(ElementName = "team_stats", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class TeamStats
{

    [XmlElement(ElementName = "coverage_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string CoverageType { get; set; }

    [XmlElement(ElementName = "season", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? Season { get; set; }

    [XmlElement(ElementName = "stats", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Stats Stats { get; set; }

    [XmlElement(ElementName = "week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? Week { get; set; }

    [XmlElement(ElementName = "date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime? Date { get; set; }

}

[XmlRoot(ElementName = "team_points", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class TeamPoints
{

    [XmlElement(ElementName = "coverage_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string CoverageType { get; set; }

    [XmlElement(ElementName = "season", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? Season { get; set; }

    [XmlElement(ElementName = "total", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required object Total { get; set; }

    [XmlElement(ElementName = "week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? Week { get; set; }
}

[XmlRoot(ElementName = "outcome_totals", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class OutcomeTotals
{

    [XmlElement(ElementName = "wins", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Wins { get; set; }

    [XmlElement(ElementName = "losses", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Losses { get; set; }

    [XmlElement(ElementName = "ties", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Ties { get; set; }

    [XmlElement(ElementName = "percentage", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public double Percentage { get; set; }
}

[XmlRoot(ElementName = "team_standings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class TeamStandings
{

    [XmlElement(ElementName = "rank", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Rank { get; set; }

    [XmlElement(ElementName = "playoff_seed", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int PlayoffSeed { get; set; }

    [XmlElement(ElementName = "outcome_totals", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required OutcomeTotals OutcomeTotals { get; set; }

    [XmlElement(ElementName = "games_back", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string GamesBack { get; set; }
}

[XmlRoot(ElementName = "team", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Team
{

    [XmlElement(ElementName = "team_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string TeamKey { get; set; }

    [XmlElement(ElementName = "team_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int TeamId { get; set; }

    [XmlElement(ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Name { get; set; }

    [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Url { get; set; }

    [XmlElement(ElementName = "team_logos", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required TeamLogos TeamLogos { get; set; }

    [XmlElement(ElementName = "waiver_priority", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int WaiverPriority { get; set; }

    [XmlElement(ElementName = "number_of_moves", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int NumberOfMoves { get; set; }

    [XmlElement(ElementName = "number_of_trades", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int NumberOfTrades { get; set; }

    [XmlElement(ElementName = "roster_adds", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required RosterAdds RosterAdds { get; set; }

    [XmlElement(ElementName = "league_scoring_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string LeagueScoringType { get; set; }

    [XmlElement(ElementName = "has_draft_grade", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int HasDraftGrade { get; set; }

    [XmlElement(ElementName = "managers", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Managers Managers { get; set; }

    [XmlElement(ElementName = "team_stats", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public TeamStats? TeamStats { get; set; }

    [XmlElement(ElementName = "team_points", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public TeamPoints? TeamPoints { get; set; }

    [XmlElement(ElementName = "team_standings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public TeamStandings? TeamStandings { get; set; }

    [XmlElement(ElementName = "is_owned_by_current_login", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? IsOwnedByCurrentLogin { get; set; }

    [XmlElement(ElementName = "team_remaining_games", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public TeamRemainingGames? TeamRemainingGames { get; set; }

    [XmlElement(ElementName = "matchups", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Matchups? Matchups { get; set; }

    [XmlElement(ElementName = "roster", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Roster? Roster { get; set; }
}

[XmlRoot(ElementName = "teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Teams
{

    [XmlElement(ElementName = "team", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<Team> Team { get; set; }

    [XmlAttribute(AttributeName = "count", Namespace = "")]
    public int Count { get; set; }

    [XmlText]
    public required string Text { get; set; }
}

[XmlRoot(ElementName = "standings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Standings
{

    [XmlElement(ElementName = "teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Teams Teams { get; set; }
}

[XmlRoot(ElementName = "stat_winner", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class StatWinner
{

    [XmlElement(ElementName = "stat_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int StatId { get; set; }

    [XmlElement(ElementName = "winner_team_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string WinnerTeamKey { get; set; }

    [XmlElement(ElementName = "is_tied", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? IsTied { get; set; }
}

[XmlRoot(ElementName = "stat_winners", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class StatWinners
{

    [XmlElement(ElementName = "stat_winner", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<StatWinner> StatWinner { get; set; }
}

[XmlRoot(ElementName = "total", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Total
{

    [XmlElement(ElementName = "remaining_games", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int RemainingGames { get; set; }

    [XmlElement(ElementName = "live_games", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int LiveGames { get; set; }

    [XmlElement(ElementName = "completed_games", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int CompletedGames { get; set; }
}

[XmlRoot(ElementName = "team_remaining_games", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class TeamRemainingGames
{

    [XmlElement(ElementName = "coverage_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string CoverageType { get; set; }

    [XmlElement(ElementName = "week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Week { get; set; }

    [XmlElement(ElementName = "total", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Total Total { get; set; }
}

[XmlRoot(ElementName = "matchup", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Matchup
{

    [XmlElement(ElementName = "week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Week { get; set; }

    [XmlElement(ElementName = "week_start", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime WeekStart { get; set; }

    [XmlElement(ElementName = "week_end", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime WeekEnd { get; set; }

    [XmlElement(ElementName = "status", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Status { get; set; }

    [XmlElement(ElementName = "is_playoffs", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsPlayoffs { get; set; }

    [XmlElement(ElementName = "is_consolation", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsConsolation { get; set; }

    [XmlElement(ElementName = "stat_winners", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required StatWinners StatWinners { get; set; }

    [XmlElement(ElementName = "teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Teams Teams { get; set; }

    [XmlElement(ElementName = "winner_team_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? WinnerTeamKey { get; set; }
}

[XmlRoot(ElementName = "matchups", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Matchups
{

    [XmlElement(ElementName = "matchup", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<Matchup> Matchup { get; set; }

    [XmlAttribute(AttributeName = "count", Namespace = "")]
    public int Count { get; set; }

    [XmlText]
    public required string Text { get; set; }
}

[XmlRoot(ElementName = "scoreboard", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Scoreboard
{

    [XmlElement(ElementName = "week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Week { get; set; }

    [XmlElement(ElementName = "matchups", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Matchups Matchups { get; set; }
}

[XmlRoot(ElementName = "league", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class League
{

    [XmlElement(ElementName = "league_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string LeagueKey { get; set; }

    [XmlElement(ElementName = "league_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int LeagueId { get; set; }

    [XmlElement(ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Name { get; set; }

    [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Url { get; set; }

    [XmlElement(ElementName = "logo_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string LogoUrl { get; set; }

    [XmlElement(ElementName = "password", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required object Password { get; set; }

    [XmlElement(ElementName = "draft_status", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string DraftStatus { get; set; }

    [XmlElement(ElementName = "num_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int NumTeams { get; set; }

    [XmlElement(ElementName = "edit_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime EditKey { get; set; }

    [XmlElement(ElementName = "weekly_deadline", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string WeeklyDeadline { get; set; }

    [XmlElement(ElementName = "league_update_timestamp", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int LeagueUpdateTimestamp { get; set; }

    [XmlElement(ElementName = "scoring_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string ScoringType { get; set; }

    [XmlElement(ElementName = "league_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string LeagueType { get; set; }

    [XmlElement(ElementName = "renew", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Renew { get; set; }

    [XmlElement(ElementName = "renewed", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required object Renewed { get; set; }

    [XmlElement(ElementName = "felo_tier", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string FeloTier { get; set; }

    [XmlElement(ElementName = "iris_group_chat_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required object IrisGroupChatId { get; set; }

    [XmlElement(ElementName = "short_invitation_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string ShortInvitationUrl { get; set; }

    [XmlElement(ElementName = "allow_add_to_dl_extra_pos", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int AllowAddToDlExtraPos { get; set; }

    [XmlElement(ElementName = "is_pro_league", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsProLeague { get; set; }

    [XmlElement(ElementName = "is_cash_league", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsCashLeague { get; set; }

    [XmlElement(ElementName = "current_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int CurrentWeek { get; set; }

    [XmlElement(ElementName = "start_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int StartWeek { get; set; }

    [XmlElement(ElementName = "start_date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime StartDate { get; set; }

    [XmlElement(ElementName = "end_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int EndWeek { get; set; }

    [XmlElement(ElementName = "end_date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime EndDate { get; set; }

    [XmlElement(ElementName = "is_plus_league", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsPlusLeague { get; set; }

    [XmlElement(ElementName = "game_code", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string GameCode { get; set; }

    [XmlElement(ElementName = "season", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Season { get; set; }

    [XmlElement(ElementName = "settings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Settings? Settings { get; set; }

    [XmlElement(ElementName = "standings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Standings? Standings { get; set; }

    [XmlElement(ElementName = "scoreboard", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Scoreboard? Scoreboard { get; set; }

    [XmlElement(ElementName = "is_finished", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? IsFinished { get; set; }
}

[XmlRoot(ElementName = "fantasy_content", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class FantasyContent
{

    [XmlElement(ElementName = "league", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public League? League { get; set; }

    [XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
    public required string Lang { get; set; }

    [XmlAttribute(AttributeName = "uri", Namespace = "http://www.yahooapis.com/v1/base.rng")]
    public required string Uri { get; set; }

    [XmlAttribute(AttributeName = "time", Namespace = "")]
    public required string Time { get; set; }

    [XmlAttribute(AttributeName = "copyright", Namespace = "")]
    public required string Copyright { get; set; }

    [XmlAttribute(AttributeName = "refresh_rate", Namespace = "")]
    public int RefreshRate { get; set; }

    [XmlAttribute(AttributeName = "yahoo", Namespace = "http://www.w3.org/2000/xmlns/")]
    public required string Yahoo { get; set; }

    [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
    public required string Xmlns { get; set; }

    [XmlText]
    public required string Text { get; set; }

    [XmlElement(ElementName = "team", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Team? Team { get; set; }

    [XmlElement(ElementName = "player", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Player? Player { get; set; }

    [XmlElement(ElementName = "users", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Users? Users { get; set; }

}

[XmlRoot(ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Name
{

    [XmlElement(ElementName = "full", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Full { get; set; }

    [XmlElement(ElementName = "first", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string First { get; set; }

    [XmlElement(ElementName = "last", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Last { get; set; }

    [XmlElement(ElementName = "ascii_first", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string AsciiFirst { get; set; }

    [XmlElement(ElementName = "ascii_last", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string AsciiLast { get; set; }
}

[XmlRoot(ElementName = "is_keeper", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class IsKeeper
{

    [XmlElement(ElementName = "status", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required object Status { get; set; }

    [XmlElement(ElementName = "cost", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required object Cost { get; set; }

    [XmlElement(ElementName = "kept", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public object? Kept { get; set; }
}

[XmlRoot(ElementName = "headshot", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Headshot
{

    [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Url { get; set; }

    [XmlElement(ElementName = "size", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Size { get; set; }
}

[XmlRoot(ElementName = "eligible_positions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class EligiblePositions
{

    [XmlElement(ElementName = "position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<string> Position { get; set; }
}

[XmlRoot(ElementName = "eligible_positions_to_add", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class EligiblePositionsToAdd
{

    [XmlElement(ElementName = "position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required List<string> Position { get; set; }
}

[XmlRoot(ElementName = "selected_position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class SelectedPosition
{

    [XmlElement(ElementName = "coverage_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string CoverageType { get; set; }

    [XmlElement(ElementName = "date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime Date { get; set; }

    [XmlElement(ElementName = "position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string Position { get; set; }

    [XmlElement(ElementName = "is_flex", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsFlex { get; set; }
}

[XmlRoot(ElementName = "player", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Player
{

    [XmlElement(ElementName = "player_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string PlayerKey { get; set; }

    [XmlElement(ElementName = "player_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int PlayerId { get; set; }

    [XmlElement(ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Name Name { get; set; }

    [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string Url { get; set; }

    [XmlElement(ElementName = "editorial_player_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string EditorialPlayerKey { get; set; }

    [XmlElement(ElementName = "editorial_team_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string EditorialTeamKey { get; set; }

    [XmlElement(ElementName = "editorial_team_full_name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string EditorialTeamFullName { get; set; }

    [XmlElement(ElementName = "editorial_team_abbr", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string EditorialTeamAbbr { get; set; }

    [XmlElement(ElementName = "editorial_team_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string EditorialTeamUrl { get; set; }

    [XmlElement(ElementName = "is_keeper", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required IsKeeper IsKeeper { get; set; }

    [XmlElement(ElementName = "uniform_number", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int UniformNumber { get; set; }

    [XmlElement(ElementName = "display_position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string DisplayPosition { get; set; }

    [XmlElement(ElementName = "headshot", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required Headshot Headshot { get; set; }

    [XmlElement(ElementName = "image_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string ImageUrl { get; set; }

    [XmlElement(ElementName = "is_undroppable", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsUndroppable { get; set; }

    [XmlElement(ElementName = "position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required string PositionType { get; set; }

    [XmlElement(ElementName = "primary_position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? PrimaryPosition { get; set; }

    [XmlElement(ElementName = "eligible_positions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required EligiblePositions EligiblePositions { get; set; }

    [XmlElement(ElementName = "eligible_positions_to_add", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public required EligiblePositionsToAdd EligiblePositionsToAdd { get; set; }

    [XmlElement(ElementName = "has_player_notes", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int HasPlayerNotes { get; set; }

    [XmlElement(ElementName = "player_notes_last_timestamp", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int PlayerNotesLastTimestamp { get; set; }

    [XmlElement(ElementName = "selected_position", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public SelectedPosition? SelectedPosition { get; set; }

    [XmlElement(ElementName = "has_recent_player_notes", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int? HasRecentPlayerNotes { get; set; }

    [XmlElement(ElementName = "status", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? Status { get; set; }

    [XmlElement(ElementName = "status_full", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? StatusFull { get; set; }

    [XmlElement(ElementName = "injury_note", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string? InjuryNote { get; set; }

}

[XmlRoot(ElementName = "players", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Players
{

    [XmlElement(ElementName = "player", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public List<Player> Player { get; set; }

    [XmlAttribute(AttributeName = "count", Namespace = "")]
    public int Count { get; set; }

    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "roster", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Roster
{

    [XmlElement(ElementName = "coverage_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string CoverageType { get; set; }

    [XmlElement(ElementName = "date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public DateTime Date { get; set; }

    [XmlElement(ElementName = "is_prescoring", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsPrescoring { get; set; }

    [XmlElement(ElementName = "is_editable", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsEditable { get; set; }

    [XmlElement(ElementName = "players", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Players Players { get; set; }
}
[XmlRoot(ElementName = "game", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Game
{

    [XmlElement(ElementName = "game_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int GameKey { get; set; }

    [XmlElement(ElementName = "game_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int GameId { get; set; }

    [XmlElement(ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string Name { get; set; }

    [XmlElement(ElementName = "code", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string Code { get; set; }

    [XmlElement(ElementName = "type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string Type { get; set; }

    [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string Url { get; set; }

    [XmlElement(ElementName = "season", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int Season { get; set; }

    [XmlElement(ElementName = "is_registration_over", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsRegistrationOver { get; set; }

    [XmlElement(ElementName = "is_game_over", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsGameOver { get; set; }

    [XmlElement(ElementName = "is_offseason", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsOffseason { get; set; }

    [XmlElement(ElementName = "leagues", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Leagues Leagues { get; set; }

    [XmlElement(ElementName = "is_live_draft_lobby_active", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public int IsLiveDraftLobbyActive { get; set; }
}

[XmlRoot(ElementName = "leagues", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Leagues
{

    [XmlElement(ElementName = "league", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public List<League> League { get; set; }

    [XmlAttribute(AttributeName = "count", Namespace = "")]
    public int Count { get; set; }

    [XmlText]
    public string Text { get; set; }
}


[XmlRoot(ElementName = "games", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Games
{

    [XmlElement(ElementName = "game", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public List<Game> Game { get; set; }

    [XmlAttribute(AttributeName = "count", Namespace = "")]
    public int Count { get; set; }

    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "user", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class User
{

    [XmlElement(ElementName = "guid", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public string Guid { get; set; }

    [XmlElement(ElementName = "games", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public Games Games { get; set; }
}

[XmlRoot(ElementName = "users", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
public class Users
{

    [XmlElement(ElementName = "user", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public User User { get; set; }

    [XmlAttribute(AttributeName = "count", Namespace = "")]
    public int Count { get; set; }

    [XmlText]
    public string Text { get; set; }
}

