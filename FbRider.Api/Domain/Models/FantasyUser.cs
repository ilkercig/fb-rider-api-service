namespace FbRider.Api.Domain.Models
{
    public class FantasyUser
    {
        public required IEnumerable<FantasySeason> Seasons { get; init; }

        public required string Key { get; init; }
    }
}
