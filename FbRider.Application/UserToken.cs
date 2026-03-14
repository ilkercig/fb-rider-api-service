namespace FbRider.Application;

public record UserToken(
    string Email,
    string AccessToken,
    string? RefreshToken,
    DateTimeOffset TokenExpiration
);
