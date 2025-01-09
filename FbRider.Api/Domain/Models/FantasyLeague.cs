namespace FbRider.Api.Domain.Models
{
    public class FantasyLeague
    {
        public required string Key { get; init; }
        public required string Id { get; init; }
        public required string Name { get; init; }
        public string? LogoUrl { get; init; }
        public required ScoringType ScoringType { get; init; }
        public required int CurrentWeek { get; init; }
        public required int StartWeek { get; init; }
        public IEnumerable<FantasyTeam>? Teams { get; set; }
        public LeagueSettings? Settings { get; set; }
    }
}
