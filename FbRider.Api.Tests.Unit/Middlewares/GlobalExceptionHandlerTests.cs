using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using FbRider.Api.Middlewares;
using FbRider.Api.DTOs;
using FbRider.YahooApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FbRider.Api.Tests.Unit.Middlewares;

[TestFixture]
public class GlobalExceptionHandlerTests
{
    private Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private DefaultHttpContext _httpContext;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

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
        var exception = new YahooSignInException("A network error", "/test", string.Empty, HttpStatusCode.InternalServerError, innerException: new HttpRequestException("some error"));
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response!.Title);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Detail);
    }

    [Test]
    public async Task TryHandleAsync_YahooSignInApiException_BadRequest_ReturnsExpectedResponse()
    {
        var oAuthError = new OAuthErrorResponse("invalid_request", "Invalid request description");
        // Arrange
        var exception = new YahooSignInException("Bad request", "/test", JsonSerializer.Serialize(oAuthError), HttpStatusCode.BadRequest);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(oAuthError.Error, response!.Title);
        Assert.AreEqual(oAuthError.ErrorDescription, response.Detail);
    }

    [Test]
    public async Task TryHandleAsync_YahooSignInApiException_BadRequestWithBadErrorResponse_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooSignInException("Bad request", "/test", "invalid_request", HttpStatusCode.BadRequest);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(GlobalExceptionHandler.OAuthErrorTitle, response!.Title);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Detail);
    }

    [Test]
    public async Task TryHandleAsync_YahooSignInApiException_ServiceUnavailable_ReturnsExpectedResponse()
    {
        // Arrange
        var oAuthError = new OAuthErrorResponse("invalid_request", "Invalid request description");
        var exception = new YahooSignInException("Server error", "/test", JsonSerializer.Serialize(oAuthError), HttpStatusCode.ServiceUnavailable);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response!.Title);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Detail);
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
        var exception = new YahooFantasySportsException("Bad request", "/test", stringWriter.ToString(), HttpStatusCode.BadRequest);

        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(apiError.Description, response!.Title);
        Assert.AreEqual(apiError.Detail, response.Detail);
    }

    [Test]
    public async Task TryHandleAsync_YahooFantasySportsApiException_BadRequestBadErrorResponse_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooFantasySportsException("Bad request", "/test", "some kind of error", HttpStatusCode.BadRequest);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response!.Title);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Detail);
    }

    [Test]
    public async Task TryHandleAsync_YahooFantasySportsApiException_InternalServerError_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooFantasySportsException("Server error", "/test", "Unexpected error", HttpStatusCode.InternalServerError);
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response!.Title);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorMessage, response.Detail);
    }

    [Test]
    public async Task TryHandleAsync_YahooApiValidationException_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooApiValidationException("Validation error message", "/test");
        var handler = CreateHandler();

        // Act
        var result = await handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(GlobalExceptionHandler.YahooApiErrorTitle, response!.Title);
        Assert.AreEqual("Validation error message", response.Detail);
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
        var response = JsonSerializer.Deserialize<ProblemDetails>(ReadResponseBody(_httpContext), JsonOptions);
        Assert.AreEqual(GlobalExceptionHandler.GenericErrorTitle, response!.Title);
        Assert.AreEqual(GlobalExceptionHandler.GenericErrorMessage, response.Detail);
    }
}
