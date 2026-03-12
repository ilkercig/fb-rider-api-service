using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FbRider.Api.Models
{
    public class UserToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Email")]
        public required string Email { get; set; }

        public required string AccessToken { get; set; }

        public required string RefreshToken { get; set; }

        public required DateTimeOffset TokenExpiration { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}