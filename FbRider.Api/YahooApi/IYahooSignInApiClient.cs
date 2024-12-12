using FbRider.Api.DTOs;

namespace FbRider.Api.YahooApi;

public interface IYahooSignInApiClient
{
    Task<TokenResponse> GetAccessToken(string code);
    Task<YahooUser> GetCurrentUser(string accessToken);
    Task<TokenResponse> GetAccessTokenByRefreshToken(string refreshToken);
}