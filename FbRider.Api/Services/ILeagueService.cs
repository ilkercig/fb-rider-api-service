using FbRider.Api.DTOs.Resources;

namespace FbRider.Api.Services;

public interface ILeagueService
{
    Task<League> GetLeagueInfoAsync(string accessToken, string leagueKey);
}