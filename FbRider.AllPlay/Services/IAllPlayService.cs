using FbRider.Api.Domain.Models;
using FbRider.AllPlay.DTOs;

namespace FbRider.AllPlay.Services;

public interface IAllPlayService
{
    Task<AllPlayStandingsDTO> GetTeamAllPlaySeasonScores(string accessToken, string leagueKey);
}
