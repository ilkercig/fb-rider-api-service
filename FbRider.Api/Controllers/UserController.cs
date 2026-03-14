using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using FbRider.Api.Responses;
using FbRider.Api.Utils;
using FbRider.Application;
using FbRider.Application.Services;
using FbRider.Domain.Models;

namespace FbRider.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class UserController(ILeagueService leagueService, IUserService userService) : ControllerBase
{
    [HttpGet("seasons")]
    [ProducesResponseType(typeof(SeasonResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SeasonResponse[]>> GetUserSeasons(CancellationToken cancellationToken)
    {
        var userToken = await GetUserTokenAsync();
        var seasons = await leagueService.GetUserSeasonsAsync(userToken.AccessToken);
        return Ok(seasons.Select(s => s.ToResponse()).ToArray());
    }

    [HttpGet("leagues")]
    [ProducesResponseType(typeof(IList<LeagueResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IList<LeagueResponse>>> GetUserLeagues(
        [FromQuery][BindRequired] string scoring,
        CancellationToken cancellationToken)
    {
        var scoringType = EnumConvertor.GetScoringType(scoring);
        if (scoringType == ScoringType.Unknown)
            return Problem(
                detail: $"'{scoring}' is not a valid scoring type.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Scoring Type");

        var userToken = await GetUserTokenAsync();
        var leagues = await leagueService.GetUserActiveLeaguesAsync(userToken.AccessToken, scoringType);
        return Ok(leagues.Select(l => l.ToResponse()).ToList());
    }

    [HttpGet("leagues/{leagueKey}/team")]
    [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TeamResponse>> GetUserTeamByLeague(string leagueKey, CancellationToken cancellationToken)
    {
        var userToken = await GetUserTokenAsync();
        var userTeam = await leagueService.GetUserTeamByLeagueAsync(userToken.AccessToken, leagueKey);
        return Ok(userTeam.ToResponse());
    }

    private async Task<UserToken> GetUserTokenAsync()
    {
        var userEmail = User.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
        return await userService.GetUserTokenAsync(userEmail);
    }
}
