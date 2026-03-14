namespace FbRider.Application;

public record BearerToken(string AccessToken, string? RefreshToken, string TokenType, int ExpiresIn, string? IdToken);
