using FbRider.Api.DTOs.Resources;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Services;

public class LeagueService(IYahooFantasySportsApiClient yahooFantasySportsApiClient) : ILeagueService
{
    public async Task<League> GetLeagueInfoAsync(string accessToken, string leagueKey)
    {
        return await yahooFantasySportsApiClient.GetLeague(accessToken, leagueKey);
    }
}