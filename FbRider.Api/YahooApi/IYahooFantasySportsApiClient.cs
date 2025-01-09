namespace FbRider.Api.YahooApi;

public interface IYahooFantasySportsApiClient
{
    Task<League> GetLeague(string accessToken, string leagueKey);

    Task<Game[]> GetUserGames(string accessToken);

    Task<Team> GetTeam(string accessToken, string teamKey);

    Task<TeamStatsResource> GetTeamStatsByWeek(string accessToken, string teamKey, int week);
}