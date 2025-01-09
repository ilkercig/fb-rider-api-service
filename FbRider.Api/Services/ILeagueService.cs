using FbRider.Api.Domain.Models;

namespace FbRider.Api.Services;

public interface ILeagueService
{
    Task<FantasySeason[]> GetUserSeasonsAsync(string accessToken);

    Task<FantasyLeague[]> GetUserActiveLeaguesAsync(string accessToken, ScoringType scoringType);
    Task<FantasyTeam>  GetUserTeamByLeagueAsync(string accessToken, string leagueKey);


    Task<FantasyLeague> GetLeagueAsync(string accessToken, string leagueKey);
}