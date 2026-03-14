namespace FbRider.Domain.Models
{
    public class Season
    {
        public required string Key { get; init; }
        public required int SeasonYear { get; init; }
        public bool IsSeasonOver { get; set; }
        public required IEnumerable<League> Leagues { get; init; }
    }
}
