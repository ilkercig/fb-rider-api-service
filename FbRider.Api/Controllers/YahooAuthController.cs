using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FbRider.Api.DTOs;
using FbRider.Api.Models;
using FbRider.Api.Services;
using FbRider.Api.YahooApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FbRider.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class YahooAuthController(IYahooSignInApiClient yahooSignInApiClient, IUserService userService) : ControllerBase
    {
        public const string AuthenticationCodeIsMissing = "Authentication code is missing";
        public const string NonceIsMissing = "Nonce is missing.";
        private const string InvalidIdToken = "Invalid ID token.";

        [HttpGet]
        [Authorize]
        public IActionResult Status()
        {
            var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
            return Ok(userEmail);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<YahooUser>> Me()
        {
            var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
            YahooUser user = await userService.GetYahooUserAsync(userEmail);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] CallbackRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(AuthenticationCodeIsMissing);
            }

            if (string.IsNullOrEmpty(request.Nonce))
            {
                return BadRequest(NonceIsMissing);
            }
            var token = await yahooSignInApiClient.GetAccessToken(request.Code);
            // Decode the ID token and validate claims
            var idToken = token.IdToken;
            string userEmail;
            if (idToken != null)
            {
                var validatedToken = ValidateIdToken(idToken, request.Nonce);
                if (validatedToken == null)
                {
                    return Unauthorized(InvalidIdToken);
                }
                userEmail = validatedToken.Claims.Single(c => c.Type == "email").Value;
            }
            else
            {
               userEmail = (await yahooSignInApiClient.GetCurrentUser(token.AccessToken)).Email;
            }

            
            var userToken = new UserToken
            {
                Email = userEmail,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken!,
                TokenExpiration = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn - 60)
            };
            await userService.AddOrUpdateUserTokenAsync(userToken);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, userEmail)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            return Ok(new { email = userEmail });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        private ClaimsPrincipal? ValidateIdToken(string idToken, string expectedNonce)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(idToken);
            var nonceClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "nonce");
            if (nonceClaim == null || nonceClaim.Value != expectedNonce)
            {
                return null;
            }
            return new ClaimsPrincipal(new ClaimsIdentity(jwtToken?.Claims));
        }

    }
}
