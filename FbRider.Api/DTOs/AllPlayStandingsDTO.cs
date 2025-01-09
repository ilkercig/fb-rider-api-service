using FbRider.Api.Domain.Models;

namespace FbRider.Api.DTOs
{
    public class AllPlayStandingsDTO
    {
        public required IEnumerable<StatCategory> StatCategories { get; init; }

        public required IEnumerable<TeamScoreDTO> TeamScores { get; init; }

    }

    public class TeamScoreDTO
    {
        public required string TeamKey { get; init; }
        public required string TeamName { get; init; }
        public required IReadOnlyDictionary<int, float> StatScores { get; init; }
        public float OverallScore => StatScores.Values.Sum();

    }
}
