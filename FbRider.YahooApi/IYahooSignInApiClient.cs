using FbRider.YahooApi;

namespace FbRider.YahooApi;

public interface IYahooSignInApiClient
{
    Task<TokenResponse> GetAccessToken(string code);
    Task<YahooUser> GetCurrentUser(string accessToken);
    Task<TokenResponse> GetAccessTokenByRefreshToken(string refreshToken);
}