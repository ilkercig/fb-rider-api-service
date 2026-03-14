namespace FbRider.Domain.Models
{
    public class User
    {
        public required IEnumerable<Season> Seasons { get; init; }

        public required string Key { get; init; }
    }
}
