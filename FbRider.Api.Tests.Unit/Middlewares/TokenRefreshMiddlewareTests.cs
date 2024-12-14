using System.Security.Claims;
using FbRider.Api.DTOs;
using FbRider.Api.Middlewares;
using FbRider.Api.Models;
using FbRider.Api.Services;
using FbRider.Api.YahooApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FbRider.Api.Tests.Unit.Middlewares;

[TestFixture]
public class TokenRefreshMiddlewareTests
{
    [SetUp]
    public void SetUp()
    {
        _mockYahooApiClient = new Mock<IYahooSignInApiClient>();
        _mockLogger = new Mock<ILogger<TokenRefreshMiddleware>>();
        _mockNext = new Mock<RequestDelegate>();
        _mockContext = new Mock<HttpContext>();
        _mockUser = new Mock<ClaimsPrincipal>();
        _userServiceMock = new Mock<IUserService>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        var mockAuthenticationService = new Mock<IAuthenticationService>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

        // Mock the repository
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IUserService)))
            .Returns(_userServiceMock.Object);

        // Mock the IAuthenticationService
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(mockAuthenticationService.Object);

        // Mock the IServiceScope
        mockServiceScope
            .Setup(scope => scope.ServiceProvider)
            .Returns(_serviceProviderMock.Object);

        // Mock the IServiceScopeFactory
        mockServiceScopeFactory
            .Setup(factory => factory.CreateScope())
            .Returns(mockServiceScope.Object);

        // Ensure the IServiceProvider returns the IServiceScopeFactory
        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockServiceScopeFactory.Object);

        _middleware = new TokenRefreshMiddleware(
            _mockNext.Object,
            _mockYahooApiClient.Object,
            _serviceProviderMock.Object,
            _mockLogger.Object
        );

        // Mock the HttpContext's RequestServices to return the IServiceProvider
        _mockContext.Setup(c => c.RequestServices).Returns(_serviceProviderMock.Object);
    }

    private Mock<IYahooSignInApiClient> _mockYahooApiClient;
    private Mock<IUserService> _userServiceMock;
    private Mock<ILogger<TokenRefreshMiddleware>> _mockLogger;
    private Mock<RequestDelegate> _mockNext;
    private TokenRefreshMiddleware _middleware;
    private Mock<HttpContext> _mockContext;
    private Mock<ClaimsPrincipal> _mockUser;
    private Mock<IServiceProvider> _serviceProviderMock;

    private const string DummyUserEmail = "user123@example.com";
    private const string DummyUserId = "user123";

    [Test]
    public async Task InvokeAsync_UserNotAuthenticated_SkipsTheMiddleware()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.Email, DummyUserEmail)]));
        _mockContext.Setup(c => c.User).Returns(claimsPrincipal);

        // Act & Assert
        await _middleware.InvokeAsync(_mockContext.Object);
        _userServiceMock.Verify(repo => repo.GetUserTokenAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void InvokeAsync_UserEmailClaimNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, DummyUserId)], "custom"));
        _mockContext.Setup(c => c.User).Returns(claimsPrincipal);

        // Act & Assert
        var exception =
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _middleware.InvokeAsync(_mockContext.Object));
        Assert.That(exception.Message, Contains.Substring("User email claim not found"));
    }


    [Test]
    public async Task InvokeAsync_AccessTokenIsValid_TokenNotRefreshed()
    {
        // Arrange
        var validToken = new UserToken
        {
            AccessToken = "valid-token",
            RefreshToken = "valid-refresh-token",
            TokenExpiration = DateTimeOffset.UtcNow.AddMinutes(5),
            Email = DummyUserEmail
        };

        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.Email, DummyUserEmail)], "custom"));
        _mockContext.Setup(c => c.User).Returns(claimsPrincipal);
        _userServiceMock.Setup(repo => repo.GetUserTokenAsync(DummyUserEmail)).ReturnsAsync(validToken);

        // Act
        await _middleware.InvokeAsync(_mockContext.Object);

        // Assert
        _userServiceMock.Verify(s => s.GetUserTokenAsync(DummyUserEmail), Times.Once);
        _mockYahooApiClient.Verify(api => api.GetAccessTokenByRefreshToken(It.IsAny<string>()), Times.Never);
        _userServiceMock.Verify(repo => repo.AddOrUpdateUserTokenAsync(It.IsAny<UserToken>()), Times.Never);
    }

    [Test]
    public async Task InvokeAsync_AccessTokenExpired_TokenRefreshed()
    {
        // Arrange
        var expiredToken = new UserToken
        {
            AccessToken = "expired-token",
            RefreshToken = "valid-refresh-token",
            TokenExpiration = DateTimeOffset.UtcNow.AddMinutes(-1), // Token expired
            Email = DummyUserEmail
        };

        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.Email, DummyUserEmail)], "custom"));
        _mockContext.Setup(c => c.User).Returns(claimsPrincipal);
        _userServiceMock.Setup(repo => repo.GetUserTokenAsync(DummyUserEmail)).ReturnsAsync(expiredToken);

        var newAccessTokenResponse = new TokenResponse
        {
            AccessToken = "new-access-token",
            ExpiresIn = 3600,
            RefreshToken = "new-refresh-token",
            TokenType = "Bearer",
            IdToken = "someIdToken"
        };

        _mockYahooApiClient.Setup(api => api.GetAccessTokenByRefreshToken("valid-refresh-token"))
            .ReturnsAsync(newAccessTokenResponse);

        // Act
        await _middleware.InvokeAsync(_mockContext.Object);

        // Assert
        _userServiceMock.Verify(
            repo => repo.AddOrUpdateUserTokenAsync(It.Is<UserToken>(u => u.AccessToken == "new-access-token")),
            Times.Once);
        _mockYahooApiClient.Verify(api => api.GetAccessTokenByRefreshToken("valid-refresh-token"), Times.Once);
    }

    //TODO: Missing test: YahooApi returns Forbidden or Unauthorized, return 401 and sign out the user

}