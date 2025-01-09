namespace FbRider.Api.Domain.Models
{
    public class LeagueSettings
    {
        public required IEnumerable<StatCategory> StatCategories { get; init; }

        public required IEnumerable<RosterPosition> RosterPositions { get; init; }
    }
}
