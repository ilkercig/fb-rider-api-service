using System.Security.Claims;
using FbRider.Api.Services;
using FbRider.Api.YahooApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FbRider.Api.Middlewares;
public class TokenRefreshMiddleware(
    RequestDelegate next,
    IYahooSignInApiClient yahooSignInApiClient,
    IServiceProvider serviceProvider,
    ILogger<TokenRefreshMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null) throw new ArgumentNullException(nameof(context), "User email claim not found");

            using var scope = serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var userToken = await userService.GetUserTokenAsync(userEmail);

            if (DateTimeOffset.UtcNow > userToken.TokenExpiration)
            {
                var newTokenResponse = await yahooSignInApiClient.GetAccessTokenByRefreshToken(userToken.RefreshToken);
                userToken.AccessToken = newTokenResponse.AccessToken;
                userToken.TokenExpiration = DateTime.UtcNow.AddSeconds(newTokenResponse.ExpiresIn - 60);

                if (!string.IsNullOrEmpty(newTokenResponse.RefreshToken))
                    userToken.RefreshToken = newTokenResponse.RefreshToken;

                await userService.AddOrUpdateUserTokenAsync(userToken);

                var claimsIdentity = (ClaimsIdentity)context.User.Identity;
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                logger.LogInformation($"Refreshing token for user {userEmail}");
            }
        }

        await next(context);
    }
}