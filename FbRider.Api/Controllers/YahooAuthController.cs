using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FbRider.Api.DTOs;
using FbRider.Application;
using FbRider.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FbRider.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Produces("application/json")]
public class YahooAuthController(ISignInApiClient yahooSignInApiClient, IUserService userService) : ControllerBase
{
    private const string InvalidIdToken = "Invalid ID token.";

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Status()
    {
        return Ok(GetUserEmail());
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfile>> Me(CancellationToken cancellationToken)
    {
        var user = await userService.GetUserProfileAsync(GetUserEmail());
        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Callback([FromBody] CallbackRequest request, CancellationToken cancellationToken)
    {
        var token = await yahooSignInApiClient.GetAccessToken(request.Code);
        var idToken = token.IdToken;
        string userEmail;
        if (idToken != null)
        {
            var validatedToken = ValidateIdToken(idToken, request.Nonce);
            if (validatedToken == null)
                return Problem(detail: InvalidIdToken, statusCode: StatusCodes.Status401Unauthorized, title: "Unauthorized");

            userEmail = validatedToken.Claims.Single(c => c.Type == "email").Value;
        }
        else
        {
            userEmail = (await yahooSignInApiClient.GetCurrentUser(token.AccessToken)).Email;
        }

        var userToken = new UserToken(
            userEmail,
            token.AccessToken,
            token.RefreshToken!,
            DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn - 60));
        await userService.AddOrUpdateUserTokenAsync(userToken);

        var claims = new List<Claim> { new(ClaimTypes.Email, userEmail) };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return Ok(new { email = userEmail });
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    private string GetUserEmail() =>
        User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;

    private ClaimsPrincipal? ValidateIdToken(string idToken, string expectedNonce)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(idToken);
        var nonceClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "nonce");
        if (nonceClaim == null || nonceClaim.Value != expectedNonce)
            return null;

        return new ClaimsPrincipal(new ClaimsIdentity(jwtToken?.Claims));
    }
}
