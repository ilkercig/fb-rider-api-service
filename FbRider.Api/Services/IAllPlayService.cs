using FbRider.Api.Domain.Models;
using FbRider.Api.DTOs;

namespace FbRider.Api.Services
{
    public interface IAllPlayService
    {
        Task<AllPlayStandingsDTO> GetTeamAllPlaySeasonScores(string accessToken, string leagueKey);
    }
}
