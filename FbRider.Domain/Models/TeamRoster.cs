namespace FbRider.Domain.Models
{
    public class TeamRoster
    {
        public required IEnumerable<Player> Players { get; init; }
    }
}
