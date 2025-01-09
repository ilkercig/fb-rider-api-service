namespace FbRider.Api.Domain.Models
{
    public class Manager
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public bool IsCurrentLogin { get; init; }
        public bool IsCommissioner { get; init; }
    }
}
