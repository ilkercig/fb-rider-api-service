namespace FbRider.Api.Domain.Models
{
    public class FantasyTeamRoster
    {
        public required IEnumerable<FantasyPlayer> Players { get; init; }
    }
}
