using System.ComponentModel;
using FbRider.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FbRider.Api.Utils;
using FbRider.Api.Domain.Models;

namespace FbRider.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(ILeagueService leagueService, IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("seasons")]
    public async Task<ActionResult<FantasySeason[]>> GetUserSeasons()
    {
        // Retrieve the authenticated user's email
        var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
        var userToken = await userService.GetUserTokenAsync(userEmail);

        var seasons = await leagueService.GetUserSeasonsAsync(userToken.AccessToken);
        return Ok(seasons);
    }

    [Authorize]
    [HttpGet("leagues")]
    public async Task<ActionResult<IList<FantasyLeague>>> GetUserLeagues(string scoring)
    {
        if (string.IsNullOrWhiteSpace(scoring)) return BadRequest("Scoring type is required");
        var scoringType = EnumConvertor.GetScoringType(scoring);
        if (scoringType == ScoringType.Unknown) return BadRequest("Unknown scoring type");

        // Retrieve the authenticated user's email
        var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
        var userToken = await userService.GetUserTokenAsync(userEmail);

        var leagues = await leagueService.GetUserActiveLeaguesAsync(userToken.AccessToken, scoringType);
        return Ok(leagues);
    }

    [Authorize]
    [HttpGet("leagues/{leagueKey}/team")]
    public async Task<ActionResult<FantasyTeam>> GetUserTeamByLeague(string leagueKey)
    {
        if (string.IsNullOrWhiteSpace(leagueKey)) return BadRequest("League key is required.");

        // Retrieve the authenticated user's email
        var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
        var userToken = await userService.GetUserTokenAsync(userEmail);

        var userTeam = await leagueService.GetUserTeamByLeagueAsync(userToken.AccessToken, leagueKey);
        return Ok(userTeam);
    }
}