using FbRider.Api.DTOs.Resources;

namespace FbRider.Api.YahooApi;

public interface IYahooFantasySportsApiClient
{
    Task<League> GetLeague(string accessToken, string leagueKey);
}