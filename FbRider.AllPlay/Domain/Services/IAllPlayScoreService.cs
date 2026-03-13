using FbRider.Api.Domain.Models;
using FbRider.AllPlay.Domain.Models;

namespace FbRider.AllPlay.Domain.Services;

public interface IAllPlayScoreService
{
    IEnumerable<AllPlayMatchUp> GetMatchUps(Dictionary<string, TeamStats> weeklyTeamStats);
    Dictionary<string, AllPlayScore> GetWeeklyTeamScores(Dictionary<string, TeamStats> weeklyTeamStats);
    Dictionary<string, AllPlayScore> GetSeasonTeamScores(Dictionary<int, Dictionary<string, TeamStats>> seasonTeamStatsByWeek);
}