namespace FbRider.Api.Domain.Models
{
    public class FantasyTeam
    {
        public required string Key { get; init; }
        public required string Id { get; init; }
        public required string Name { get; init; }
        public string? TeamLogo { get; init; }
        public FantasyTeamRoster? Roster { get; set; }
        public required  IEnumerable<Manager> Managers { get; init; }
    }

}
