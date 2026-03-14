namespace FbRider.Application.Services;

public interface ISignInApiClient
{
    Task<BearerToken> GetAccessToken(string code);
    Task<UserProfile> GetCurrentUser(string accessToken);
    Task<BearerToken> GetAccessTokenByRefreshToken(string refreshToken);
}
