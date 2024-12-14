using System.Net;
using System.Text;
using System.Text.Json;
using FbRider.Api.DTOs;
using FbRider.Api.Options;
using FbRider.Api.YahooApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace FbRider.Api.Tests.Unit.YahooApi;

[TestFixture]
public class YahooSignInApiClientTests
{
    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpMessageHandlerMock
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _configurationMock = new Mock<IConfiguration>();
        _optionsMock = new Mock<IOptions<SecretsOptions>>();

        _secretsOptions = new SecretsOptions { ClientSecret = ClientSecret };
        _optionsMock.Setup(o => o.Value).Returns(_secretsOptions);

        _configurationMock.Setup(c => c["YahooAuthSettings:RedirectUri"]).Returns(RedirectUri);

        _client = new YahooSignInApiClient(_httpClient, _configurationMock.Object, _optionsMock.Object);

        // Arrange
        _tokenResponse = new TokenResponse
        {
            AccessToken = "test_access_token",
            RefreshToken = "test_refresh_token",
            ExpiresIn = 3600,
            TokenType = "Bearer",
            IdToken = "user-id-token"
        };
    }

    private HttpClient _httpClient;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;

    private Mock<IConfiguration> _configurationMock;
    private Mock<IOptions<SecretsOptions>> _optionsMock;
    private YahooSignInApiClient _client;

    private const string ValidCode = "valid-code";
    private const string ClientSecret = "test-client-secret";
    private const string RedirectUri = "https://localhost/callback";
    private SecretsOptions _secretsOptions;
    private TokenResponse _tokenResponse;

    [Test]
    public async Task GetAccessToken_ShouldSendCorrectRequestAndReturnTokenResponse_WhenRequestIsSuccessful()
    {
        var expectedFormContent = new Dictionary<string, string>
        {
            { "code", ValidCode },
            { "redirect_uri", RedirectUri },
            { "grant_type", "authorization_code" }
        };

        var authHeader =
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{YahooSignInApiClient.ClientId}:{ClientSecret}"));

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri(YahooApiUrls.TokenUrl) &&
                    req.Headers.Authorization.Scheme == "Basic" &&
                    req.Headers.Authorization.Parameter == authHeader &&
                    VerifyFormContent(req.Content, expectedFormContent)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(_tokenResponse), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.GetAccessToken(ValidCode);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.AccessToken, Is.EqualTo(_tokenResponse.AccessToken));
        Assert.That(result.RefreshToken, Is.EqualTo(_tokenResponse.RefreshToken));
        Assert.That(result.ExpiresIn, Is.EqualTo(_tokenResponse.ExpiresIn));
        Assert.That(result.TokenType, Is.EqualTo(_tokenResponse.TokenType));
    }

    [Test]
    public async Task GetRefreshToken_ShouldSendCorrectRequestAndReturnTokenResponse_WhenRequestIsSuccessful()
    {
        string refreshToken = "a_refresh_token";
        var expectedFormContent = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };


        var authHeader =
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{YahooSignInApiClient.ClientId}:{ClientSecret}"));

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri(YahooApiUrls.TokenUrl) &&
                    req.Headers.Authorization.Scheme == "Basic" &&
                    req.Headers.Authorization.Parameter == authHeader &&
                    VerifyFormContent(req.Content, expectedFormContent)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(_tokenResponse), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.GetAccessTokenByRefreshToken(refreshToken);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.AccessToken, Is.EqualTo(_tokenResponse.AccessToken));
        Assert.That(result.RefreshToken, Is.EqualTo(_tokenResponse.RefreshToken));
        Assert.That(result.ExpiresIn, Is.EqualTo(_tokenResponse.ExpiresIn));
        Assert.That(result.TokenType, Is.EqualTo(_tokenResponse.TokenType));
    }

    [Test]
    public void GetAccessToken_ShouldThrowInvalidOperationException_WhenRedirectUriIsNotConfigured()
    {
        // Arrange
        const string code = "test_code";
        _configurationMock.Setup(c => c["YahooAuthSettings:RedirectUri"]).Returns((string)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _client.GetAccessToken(code));
        Assert.That(ex.Message, Is.EqualTo(YahooSignInApiClient.RedirectUriIsNotSetInTheConfiguration));
    }

    [Test]
    public void GetAccessToken_ShouldThrowYahooApiException_WhenRequestFailsWithOAuthError()
    {
        // Arrange
        var oAuthErrorResponse = new OAuthErrorResponse("invalid_grant", "Authorization code expired");


        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri == new Uri(YahooApiUrls.TokenUrl)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(JsonSerializer.Serialize(oAuthErrorResponse), Encoding.UTF8,
                    "application/json")
            });

        // Act & Assert
        var ex = Assert.ThrowsAsync<YahooApiException>(() => _client.GetAccessToken(ValidCode));
        Assert.That(ex.Message, Does.Contain(YahooApiErrorMessages.ResponseNotSuccessful));
        Assert.That(ex.Endpoint, Is.EqualTo(YahooApiUrls.TokenUrl));
        Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(ex.ResponseContent, Is.EqualTo(JsonSerializer.Serialize(oAuthErrorResponse)));
    }

    [Test]
    public void GetAccessToken_ShouldThrowYahooApiException_OnNetworkError()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Throws(new HttpRequestException("Network error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<YahooApiException>(() => _client.GetAccessToken(ValidCode));
        Assert.That(ex.Message, Does.Contain(YahooApiErrorMessages.ApiRequestError));
        Assert.That(ex.Endpoint, Is.EqualTo(YahooApiUrls.TokenUrl));
    }

    [Test]
    public async Task GetAccessToken_ShouldThrowYahooApiException_WhenResponseIsOkAndTokenResponseDeserializationFails()
    {
        // Arrange
        const string malformedTokenResponse = "{ \"accessToken\": "; // Malformed JSON


        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post && req.RequestUri == new Uri(YahooApiUrls.TokenUrl)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(malformedTokenResponse, Encoding.UTF8, "application/json")
            });

        // Act & Assert
        var exception = Assert.ThrowsAsync<YahooApiException>(async () => await _client.GetAccessToken(ValidCode));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Does.Contain(YahooApiErrorMessages.DeserializationFailed));
        Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(exception.ResponseContent, Is.EqualTo(malformedTokenResponse));
    }

    // Helper method to verify form content
    private static bool VerifyFormContent(HttpContent content, Dictionary<string, string> expectedValues)
    {
        if (content is FormUrlEncodedContent formContent)
        {
            var actualValues = formContent.ReadAsStringAsync().Result.Split('&')
                .Select(p => p.Split('='))
                .ToDictionary(k => Uri.UnescapeDataString(k[0]), v => Uri.UnescapeDataString(v[1]));

            return expectedValues.All(kv => actualValues.ContainsKey(kv.Key) && actualValues[kv.Key] == kv.Value);
        }

        return false;
    }
}