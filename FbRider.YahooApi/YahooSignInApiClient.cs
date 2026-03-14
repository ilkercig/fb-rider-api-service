using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FbRider.Application;
using FbRider.Application.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace FbRider.YahooApi;

public class YahooSignInApiClient(HttpClient httpClient, IConfiguration configuration) : YahooApiClientBase(httpClient),
    ISignInApiClient
{
    public const string RedirectUriIsNotSetInTheConfiguration = "Redirect URI is not set in the configuration.";
    public const string ClientIdIsNotSetInTheConfiguration = "Client Id is not set in the configuration.";
    public const string ClientSecretIsNotSetInTheConfiguration = "Client Id is not set in the configuration.";

    protected override YahooApiType ApiType => YahooApiType.SignIn;

    public async Task<BearerToken> GetAccessToken(string code)
    {
        var redirectUri = configuration["YahooRedirectUri"];
        if (string.IsNullOrEmpty(redirectUri))
            throw new InvalidOperationException(RedirectUriIsNotSetInTheConfiguration);

        var formContent = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        ]);
        var yahooTokenResponse = await GetAccessToken(formContent);
        var bearerToken = new BearerToken(yahooTokenResponse.AccessToken, yahooTokenResponse.RefreshToken,yahooTokenResponse.TokenType,  yahooTokenResponse.ExpiresIn, yahooTokenResponse.IdToken);
        return bearerToken;
    }


    public async Task<BearerToken> GetAccessTokenByRefreshToken(string refreshToken)
    {
        var formContent = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        ]);
        var yahooTokenResponse = await GetAccessToken(formContent);
        var bearerToken = new BearerToken(yahooTokenResponse.AccessToken, yahooTokenResponse.RefreshToken, yahooTokenResponse.TokenType, yahooTokenResponse.ExpiresIn, yahooTokenResponse.IdToken);
        return bearerToken;
    }

    public async Task<UserProfile> GetCurrentUser(string accessToken)
    {
        var yahooUser = await GetAsync<YahooUser>($"{YahooApiUrls.UserInfoUrl}", accessToken);
        return new UserProfile(yahooUser.Email, yahooUser.Name, new Application.ProfileImages(yahooUser.ProfileImages.Image32, yahooUser.ProfileImages.Image64, yahooUser.ProfileImages.Image128));
    }

    public async Task<T> GetAsync<T>(string url, string accessToken, Dictionary<string, string>? queryParams = null)
    {
        var queryBuilder = new QueryBuilder();
        if (queryParams != null)
        {
            foreach (var kvp in queryParams) queryBuilder.Add(kvp.Key, kvp.Value);

            url = QueryHelpers.AddQueryString(url, queryBuilder.ToDictionary()!);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await SendRequest<T>(request);
    }

    private async Task<YahooTokenResponse> GetAccessToken(FormUrlEncodedContent form)
    {
        var clientId = configuration["YahooClientId"];
        if (string.IsNullOrEmpty(clientId))
            throw new InvalidOperationException(ClientIdIsNotSetInTheConfiguration);

        var clientSecret = configuration["YahooClientSecret"];
        if (string.IsNullOrEmpty(clientSecret))
            throw new InvalidOperationException(ClientSecretIsNotSetInTheConfiguration);

        var endPoint = YahooApiUrls.TokenUrl;
        var request = new HttpRequestMessage(HttpMethod.Post, endPoint);
        request.Content = form;
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        var credentials =
            Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        return await SendRequest<YahooTokenResponse>(request);
    }

   

    protected override T? Deserialize<T>(string responseContent) where T : default
    {
        return JsonSerializer.Deserialize<T>(responseContent);
    }
}
