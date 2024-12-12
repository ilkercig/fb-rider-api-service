﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FbRider.Api.DTOs;
using FbRider.Api.Options;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FbRider.Api.YahooApi;

public class YahooSignInApiClient(HttpClient httpClient, IConfiguration configuration, IOptions<SecretsOptions> options) : YahooApiClientBase(httpClient),
    IYahooSignInApiClient
{
    public const string ClientId =
        "dj0yJmk9VGRCcXN2VG1NM0pPJmQ9WVdrOVpEWmFTWEJuZURrbWNHbzlNQT09JnM9Y29uc3VtZXJzZWNyZXQmc3Y9MCZ4PTJh";

    public const string RedirectUriIsNotSetInTheConfiguration = "Redirect URI is not set in the configuration.";

    protected override YahooApiType ApiType => YahooApiType.SignIn;

    public async Task<TokenResponse> GetAccessToken(string code)
    {
        var redirectUri = configuration["YahooAuthSettings:RedirectUri"];
        if (string.IsNullOrEmpty(redirectUri))
            throw new InvalidOperationException(RedirectUriIsNotSetInTheConfiguration);

        var formContent = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        ]);
        return await GetAccessToken(formContent);
    }


    public async Task<TokenResponse> GetAccessTokenByRefreshToken(string refreshToken)
    {
        var formContent = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        ]);
        return await GetAccessToken(formContent);
    }

    public async Task<YahooUser> GetCurrentUser(string accessToken)
    {
        return await GetAsync<YahooUser>($"{YahooApiUrls.UserInfoUrl}", accessToken);
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

    private async Task<TokenResponse> GetAccessToken(FormUrlEncodedContent form)
    {
        var endPoint = YahooApiUrls.TokenUrl;
        var request = new HttpRequestMessage(HttpMethod.Post, endPoint);
        request.Content = form;
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        var credentials =
            Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{ClientId}:{options.Value.ClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        return await SendRequest<TokenResponse>(request);
    }

   

    protected override T? Deserialize<T>(string responseContent) where T : default
    {
        return JsonSerializer.Deserialize<T>(responseContent);
    }
}