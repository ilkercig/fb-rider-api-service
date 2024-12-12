using System.Text.Json.Serialization;

namespace FbRider.Api.DTOs;

public class OAuthErrorResponse(string error, string errorDescription)
{
    [JsonPropertyName("error")]
    public string? Error { get; set; } = error;
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; } = errorDescription;
}