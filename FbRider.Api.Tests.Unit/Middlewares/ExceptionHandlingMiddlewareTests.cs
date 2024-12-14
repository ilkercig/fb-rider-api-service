using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using FbRider.Api.DTOs;
using FbRider.Api.Middlewares;
using FbRider.Api.YahooApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace FbRider.Api.Tests.Unit.Middlewares;

[TestFixture]
public class ExceptionHandlingMiddlewareTests
{
    private Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
    private RequestDelegate _next;
    private DefaultHttpContext _httpContext;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _next = Mock.Of<RequestDelegate>();
        _httpContext = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };
    }

    private ExceptionHandlingMiddleware CreateMiddleware()
    {
        return new ExceptionHandlingMiddleware(_next, _loggerMock.Object);
    }

    private static string ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    [Test]
    public async Task Invoke_YahooSignInApiException_HttpException_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooApiException("A network error", "/test", YahooApiType.SignIn, innerException:new HttpRequestException("some error"));
        _next = context => throw exception;
        var middleware = CreateMiddleware();


        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        // Assert
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorMessage, response.Message);

    }

    [Test]
    public async Task Invoke_YahooSignInApiException_BadRequest_ReturnsExpectedResponse()
    {
        var oAuthError = new OAuthErrorResponse("invalid_request", "Invalid request description");
        // Arrange
        var exception = new YahooApiException("Bad request", "/test", YahooApiType.SignIn, JsonSerializer.Serialize(oAuthError), HttpStatusCode.BadRequest);
        _next = context => throw exception;
        var middleware = CreateMiddleware();


        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(oAuthError.Error, response.Error);
        Assert.AreEqual(oAuthError.ErrorDescription, response.Message);

    }

    [Test]
    public async Task Invoke_YahooSignInApiException_BadRequestWithBadErrorResponse_ReturnsExpectedResponse()
    {
        // Arrange
        var exception = new YahooApiException("Bad request", "/test", YahooApiType.SignIn, "invalid_request", HttpStatusCode.BadRequest);
        _next = context => throw exception;
        var middleware = CreateMiddleware();


        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task Invoke_YahooSignInApiException_ServiceUnavailable_ReturnsExpectedResponse()
    {
        // Arrange
        var oAuthError = new OAuthErrorResponse("invalid_request", "Invalid request description");
        var exception = new YahooApiException("Server error", "/test", YahooApiType.SignIn, JsonSerializer.Serialize(oAuthError), HttpStatusCode.ServiceUnavailable);
        _next = context => throw exception;
        var middleware = CreateMiddleware();
        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task Invoke_YahooFantasySportsApiException_BadRequest_ReturnsExpectedResponse()
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

        _next = context => throw exception;
        var middleware = CreateMiddleware();

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(apiError.Description, response.Error);
        Assert.AreEqual(apiError.Detail, response.Message);
    }

    [Test]
    public async Task Invoke_YahooFantasySportsApiException_BadRequestBadErrorResponse_ReturnsExpectedResponse()
    {

        // Arrange

        var exception = new YahooApiException("Bad request", "/test", YahooApiType.FantasySports, "some kind of error", HttpStatusCode.BadRequest);

        _next = context => throw exception;
        var middleware = CreateMiddleware();

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task Invoke_YahooFantasySportsApiException_InternalServerError_ReturnsExpectedResponse()
    {
        // Arrange
       
        var exception = new YahooApiException("Server error", "/test", YahooApiType.FantasySports, "Unexpected error", HttpStatusCode.InternalServerError);
        _next = context => throw exception;
        var middleware = CreateMiddleware();
        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorTitle, response.Error);
        Assert.AreEqual(ExceptionHandlingMiddleware.YahooApiErrorMessage, response.Message);
    }

    [Test]
    public async Task Invoke_GenericException_ReturnsInternalServerError()
    {
        // Arrange
        
        var exception = new InvalidOperationException("Something went wrong");
        _next = context => throw exception;
        var middleware = CreateMiddleware();
        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
        var responseBody = ReadResponseBody(_httpContext);
        var response = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody);
        Assert.AreEqual(ExceptionHandlingMiddleware.GenericErrorTitle, response.Error);
        Assert.AreEqual(ExceptionHandlingMiddleware.GenericErrorMessage, response.Message);
    }


}
