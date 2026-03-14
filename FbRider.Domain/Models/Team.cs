namespace FbRider.Domain.Models
{
    public class Team
    {
        public required string Key { get; init; }
        public required string Id { get; init; }
        public required string Name { get; init; }
        public string? TeamLogo { get; init; }
        public TeamRoster? Roster { get; set; }
        public required  IEnumerable<Manager> Managers { get; init; }
    }

}
