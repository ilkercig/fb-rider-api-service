using System.Security.Claims;
using FbRider.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FbRider.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaguesController(ILeagueService leagueService, IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("{leagueKey}")]
    public async Task<IActionResult> GetLeagueInfo(string leagueKey)
    {
        var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
        var userToken = await userService.GetUserTokenAsync(userEmail);
        var leagueData = await leagueService.GetLeagueInfoAsync(userToken.AccessToken, leagueKey);
        return Ok(leagueData);
    }
}