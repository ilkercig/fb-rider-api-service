using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using FbRider.Api.Controllers;
using FbRider.Api.DTOs;
using FbRider.Api.Models;
using FbRider.Api.Services;
using FbRider.Api.YahooApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace FbRider.Api.Tests.Unit.Controllers;

[TestFixture]
public class YahooAuthControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _yahooSignInApiClientMock = new Mock<IYahooSignInApiClient>();
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<YahooAuthController>>();
        _controller = new YahooAuthController(_yahooSignInApiClientMock.Object, _userServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var jwtIdToken = GenerateJwtToken(UserEmail, ValidNonce);
        _tokenResponse = new TokenResponse
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            IdToken = jwtIdToken,
            TokenType = "bearer"
        };
    }

    private Mock<IYahooSignInApiClient> _yahooSignInApiClientMock;
    private Mock<IUserService> _userServiceMock;
    private Mock<ILogger<YahooAuthController>> _loggerMock;
    private YahooAuthController _controller;
    private const string ValidNonce = "valid_nonce";
    private const string UserEmail = "user@example.com";
    private TokenResponse _tokenResponse;

    [Test]
    public async Task Callback_ShouldReturnBadRequest_WhenCodeIsMissing()
    {
        // Arrange
        var request = new CallbackRequest { Code = string.Empty, Nonce = ValidNonce };

        // Act
        var result = await _controller.Callback(request);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        Assert.AreEqual(YahooAuthController.AuthenticationCodeIsMissing, badRequest?.Value);
    }

    [Test]
    public async Task Callback_ShouldReturnBadRequest_WhenNonceIsMissing()
    {
        // Arrange
        var request = new CallbackRequest { Code = "valid-code", Nonce = string.Empty };
        // Act
        var result = await _controller.Callback(request);
        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        Assert.AreEqual(YahooAuthController.NonceIsMissing, badRequest?.Value);
    }

    [Test]
    public async Task Callback_ShouldSignInAndReturnEmail_WhenValidCode()
    {
        // Arrange
        var request = new CallbackRequest { Code = "valid-code", Nonce = ValidNonce };

        var userToken = new UserToken
        {
            Email = UserEmail,
            AccessToken = _tokenResponse.AccessToken,
            RefreshToken = _tokenResponse.RefreshToken!,
            TokenExpiration = DateTimeOffset.UtcNow.AddSeconds(_tokenResponse.ExpiresIn - 60)
        };

        _yahooSignInApiClientMock
            .Setup(client => client.GetAccessToken(request.Code))
            .ReturnsAsync(_tokenResponse);


        _userServiceMock
            .Setup(service => service.AddOrUpdateUserTokenAsync(It.IsAny<UserToken>()))
            .Returns(Task.CompletedTask);

        var authServiceMock = new Mock<IAuthenticationService>();
        _controller.ControllerContext.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(authServiceMock.Object)
            .BuildServiceProvider();

        authServiceMock
            .Setup(auth => auth.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                null))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Callback(request);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);

        _userServiceMock.Verify(service =>
            service.AddOrUpdateUserTokenAsync(It.Is<UserToken>(ut =>
                ut.Email == userToken.Email &&
                ut.AccessToken == userToken.AccessToken &&
                ut.RefreshToken == userToken.RefreshToken &&
                ut.TokenExpiration >= userToken.TokenExpiration.AddSeconds(-5) &&
                ut.TokenExpiration <= userToken.TokenExpiration.AddSeconds(5))), Times.Once);

        authServiceMock.Verify(auth =>
            auth.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.Is<ClaimsPrincipal>(principal =>
                    principal.Identity != null &&
                    principal.Identity.IsAuthenticated &&
                    principal.HasClaim(c => c.Type == ClaimTypes.Email && c.Value == UserEmail)),
                null), Times.Once);
    }

    [Test]
    public async Task Callback_ShouldReturnUnAuthorized_WhenNonceNotMatched()
    {
        var invalidNonce = "invalid_nonce";
        var request = new CallbackRequest { Code = "valid-code", Nonce = invalidNonce };

        _yahooSignInApiClientMock
            .Setup(client => client.GetAccessToken(request.Code))
            .ReturnsAsync(_tokenResponse);

        // Act
        var result = await _controller.Callback(request);

        // Assert
        Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
    }


    [Test]
    public async Task Me_ShouldReturnYahooUser_WhenUserExists()
    {
        // Arrange
        var email = "user@example.com";
        var yahooUser = new YahooUser
        {
            Email = email,
            Name = "User Name",
            Sub = "sub-id",
            EmailVerified = true,
            ProfileImages = new ProfileImages
            {
                Image32 = "image32",
                Image64 = "image64",
                Image128 = "image128"
            }
        };

        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, email)
        }));

        _userServiceMock
            .Setup(service => service.GetYahooUserAsync(email))
            .ReturnsAsync(yahooUser);

        // Act
        var result = await _controller.Me();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result.Result); // Access `Result` to check the actual type
        var okResult = result.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<YahooUser>(okResult.Value);
        Assert.AreEqual(yahooUser, okResult.Value);
    }


    [Test]
    public void Status_ShouldReturnEmail_WhenAuthorized()
    {
        // Arrange
        var email = "user@example.com";
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, email)
        }));

        // Act
        var result = _controller.Status();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.AreEqual(email, okResult?.Value);
    }

    [Test]
    public async Task Logout_ShouldSignOutUser()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthenticationService>();
        _controller.ControllerContext.HttpContext.RequestServices = new ServiceCollection()
            .AddSingleton(authServiceMock.Object)
            .BuildServiceProvider();

        authServiceMock
            .Setup(auth =>
                auth.SignOutAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, null))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Logout();

        // Assert
        Assert.IsInstanceOf<OkResult>(result);
        authServiceMock.Verify(
            auth => auth.SignOutAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, null),
            Times.Once);
    }


    private string GenerateJwtToken(string email, string nonce)
    {
        var claims = new[]
        {
            new Claim("email", email),
            new Claim("nonce", nonce)
        };

        var keyBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes); // Fill the key array with 256 bits of random data
        }

        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            "yourIssuer",
            "yourAudience",
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}