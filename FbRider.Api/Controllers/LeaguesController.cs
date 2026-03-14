using System.Security.Claims;
using FbRider.Application;
using FbRider.Application.Services;
using FbRider.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FbRider.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class LeaguesController(ILeagueService leagueService, IUserService userService) : ControllerBase
{
    [HttpGet("{leagueKey}/settings")]
    [ProducesResponseType(typeof(LeagueSettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LeagueSettingsResponse>> GetLeagueSettings(string leagueKey, CancellationToken cancellationToken)
    {
        var userToken = await GetUserTokenAsync();
        var league = await leagueService.GetLeagueAsync(userToken.AccessToken, leagueKey);

        if (league.Settings is null)
            return NotFound();

        return Ok(league.Settings.ToResponse());
    }

    private async Task<UserToken> GetUserTokenAsync()
    {
        var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
        return await userService.GetUserTokenAsync(userEmail);
    }
}
