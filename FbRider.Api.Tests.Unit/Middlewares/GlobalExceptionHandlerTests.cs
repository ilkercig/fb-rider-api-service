using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using FbRider.Api.DTOs;
using FbRider.Api.Middlewares;
using FbRider.YahooApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace FbRider.Api.Tests.Unit.Middlewares;

[TestFixture]
public class GlobalExceptionHandlerTests
{
    private Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _httpContext = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };
    }

    private GlobalExceptionHandler CreateHandler()
    {
        return new GlobalExceptionHandler(_loggerMock.Object);
    }

    private static string ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    [Test]
    public async Task TryHandleAsync_YahooSignInApiException_HttpException_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooApiException("A network error", "/test", YahooApiType.SignIn, innerException:new HttpRequestException("some error"));
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task TryHandleAsync_YahooSignInApiException_BadRequest_ReturnsExpectedResponse()
    {
        var oAuthError = new OAuthErrorResponse("invalid_request", "Invalid request description");
        // Arrange
        var exception = new YahooApiException("Bad request", "/test", YahooApiType.SignIn, JsonSerializer.Serialize(oAuthError), HttpStatusCode.BadRequest);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(oAuthError.Error, response.Error);
        Assert.AreEqual(oAuthError.ErrorDescription, response.Message);
    }

    [Test]
    public async Task TryHandleAsync_YahooSignInApiException_BadRequestWithBadErrorResponse_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooApiException("Bad request", "/test", YahooApiType.SignIn, "invalid_request", HttpStatusCode.BadRequest);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task TryHandleAsync_YahooSignInApiException_ServiceUnavailable_ReturnsExpectedResponse()
    {
        // Arrange
        var oAuthError = new OAuthErrorResponse("invalid_request", "Invalid request description");
        var exception = new YahooApiException("Server error", "/test", YahooApiType.SignIn, JsonSerializer.Serialize(oAuthError), HttpStatusCode.ServiceUnavailable);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task TryHandleAsync_YahooFantasySportsApiException_BadRequest_ReturnsExpectedResponse()
    {
        // Arrange
        var apiError = new YahooApiError()
        {
            Description = "Invalid request",
            Detail = "Invalid request description",
        };
        var xmlSerializer = new XmlSerializer(typeof(YahooApiError));
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);
        xmlSerializer.Serialize(xmlWriter, apiError);
        var exception = new YahooApiException("Bad request", "/test", YahooApiType.FantasySports, stringWriter.ToString(), HttpStatusCode.BadRequest);

        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(apiError.Description, response.Error);
        Assert.AreEqual(apiError.Detail, response.Message);
    }

    [Test]
    public async Task TryHandleAsync_YahooFantasySportsApiException_BadRequestBadErrorResponse_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooApiException("Bad request", "/test", YahooApiType.FantasySports, "some kind of error", HttpStatusCode.BadRequest);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task TryHandleAsync_YahooFantasySportsApiException_InternalServerError_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooApiException("Server error", "/test", YahooApiType.FantasySports, "Unexpected error", HttpStatusCode.InternalServerError);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task TryHandleAsync_GenericException_ReturnsInternalServerError()
    {
        // Arrange
        var exception = new InvalidOperationException("Something went wrong");
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(GlobalExceptionHandler.GenericErrorTitle, response.Error);
        Assert.AreEqual(GlobalExceptionHandler.GenericErrorMessage, response.Message);
    }
}
