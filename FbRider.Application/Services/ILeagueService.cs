using FbRider.Domain.Models;

namespace FbRider.Application.Services;

public interface ILeagueService
{
    Task<Season[]> GetUserSeasonsAsync(string accessToken);

    Task<League[]> GetUserActiveLeaguesAsync(string accessToken, ScoringType scoringType);
    Task<Team> GetUserTeamByLeagueAsync(string accessToken, string leagueKey);


    Task<League> GetLeagueAsync(string accessToken, string leagueKey);
}
