using System.Net;
using System.Text.Json;
using FbRider.YahooApi;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Polly;
using Polly.Retry;

namespace FbRider.Api.Tests.Unit.YahooApi;

[TestFixture]
public class YahooApiClientBaseRetryTests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private TestYahooApiClient _client;

    [SetUp]
    public void Setup()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpMessageHandlerMock
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());
        
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _client = new TestYahooApiClient(_httpClient);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    private class TestYahooApiClient(HttpClient httpClient) : YahooApiClientBase<YahooFantasySportsException>(httpClient, TestPipeline)
    {
        private static readonly ResiliencePipeline<HttpResponseMessage> TestPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(response => response.StatusCode is HttpStatusCode.InternalServerError 
                                              or HttpStatusCode.BadGateway 
                                              or HttpStatusCode.ServiceUnavailable 
                                              or HttpStatusCode.GatewayTimeout 
                                              or HttpStatusCode.RequestTimeout),
                BackoffType = DelayBackoffType.Constant,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(1)
            })
            .Build();

        protected override YahooFantasySportsException CreateException(string message, string endpoint, string responseContent, HttpStatusCode statusCode, Exception? innerException = null)
        {
            return new YahooFantasySportsException(message, endpoint, responseContent, statusCode, innerException);
        }

        protected override T? Deserialize<T>(string responseContent) where T : default
        {
            return JsonSerializer.Deserialize<T>(responseContent);
        }

        public Task<T> CallSendRequest<T>(HttpRequestMessage request) => SendRequest<T>(request);
    }

    [Test]
    public async Task SendRequest_ShouldRetry_OnTransientHttpStatusCode()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var sequence = _httpMessageHandlerMock.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)) // 1st attempt
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)) // 2nd attempt
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("\"success\"")
            }); // 3rd attempt

        // Act
        var result = await _client.CallSendRequest<string>(request);

        // Assert
        Assert.That(result, Is.EqualTo("success"));
        _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(3), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public async Task SendRequest_ShouldRetry_OnHttpRequestException()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var sequence = _httpMessageHandlerMock.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Throws(new HttpRequestException("Network error")) // 1st attempt
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("\"success\"")
            }); // 2nd attempt

        // Act
        var result = await _client.CallSendRequest<string>(request);

        // Assert
        Assert.That(result, Is.EqualTo("success"));
        _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public async Task SendRequest_ShouldFail_AfterMaxRetries()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("Service Unavailable")
            });

        // Act & Assert
        var ex = Assert.ThrowsAsync<YahooFantasySportsException>(() => _client.CallSendRequest<string>(request));
        Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
        Assert.That(ex.ResponseContent, Is.EqualTo("Service Unavailable"));
        
        // Initial attempt + 3 retries = 4 total calls
        _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(4), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }
}
