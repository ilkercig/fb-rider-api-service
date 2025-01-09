namespace FbRider.Api.Domain.Models
{
    public class FantasyPlayer
    {
        public required string Key { get; init; }
        public required string Id { get; init; }
        public required string FullName { get; init; }
        public string? ImageUrl { get; init; }
        public required string[] EligiblePositions { get; init; }
        public required string[] DisplayPositions { get; init; }
        public string? SelectedPosition { get; init; }
        public required bool IsUndroppable { get; init; }
        public string? Status { get; init; }
    }
}
