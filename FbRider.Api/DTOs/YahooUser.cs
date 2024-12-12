using System.Text.Json.Serialization;

namespace FbRider.Api.DTOs
{
    public class YahooUser
    {
        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("sub")]
        public required string Sub { get; init; }

        [JsonPropertyName("email_verified")]
        public required bool EmailVerified { get; init; }

        [JsonPropertyName("profile_images")]
        public required ProfileImages ProfileImages { get; init; }
    }

    public class ProfileImages
    {
        [JsonPropertyName("image32")]
        public required string Image32 { get; init; }
        [JsonPropertyName("image64")]
        public required string Image64 { get; init; }

        [JsonPropertyName("image128")]
        public required string Image128 { get; init; }

    }

}