namespace FbRider.YahooApi;

public interface IYahooFantasySportsApiClient
{
    Task<League> GetLeagueWithAllSubresources(string accessToken, string leagueKey);

    Task<Game[]> GetUserFantasyGames(string accessToken);

    Task<Team> GetTeam(string accessToken, string teamKey);

    Task<TeamStatsResource> GetTeamStatsByWeek(string accessToken, string teamKey, int week);
}
