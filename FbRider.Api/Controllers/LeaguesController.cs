using System.Security.Claims;
using FbRider.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FbRider.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaguesController(ILeagueService leagueService, IUserService userService)
    : ControllerBase
{
    private const string LeagueKeyIsRequiredError = "League key is required.";

    [HttpGet("{leagueKey}/settings")]
    [Authorize]
    public async Task<IActionResult> GetLeagueSettings(string leagueKey)
    {
        if (string.IsNullOrWhiteSpace(leagueKey)) return BadRequest(LeagueKeyIsRequiredError);

        // Retrieve the authenticated user's email
        var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
        var userToken = await userService.GetUserTokenAsync(userEmail);

        var league = await leagueService.GetLeagueAsync(userToken.AccessToken, leagueKey);
        return Ok(league.Settings);
    }
}
