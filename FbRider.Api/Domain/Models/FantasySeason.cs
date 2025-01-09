namespace FbRider.Api.Domain.Models
{
    public class FantasySeason
    {
        public required string Key { get; init; }
        public required int Season { get; init; }
        public bool IsSeasonOver { get; set; }
        public required IEnumerable<FantasyLeague> Leagues { get; init; }
    }
}
