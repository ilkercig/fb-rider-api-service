namespace FbRider.Api.Domain.Models
{
    public class RosterPosition
    {
        public required string Position { get; init; }
        public required int Count { get; init; }
        public bool IsStartingPosition { get; init; }
    }
}
