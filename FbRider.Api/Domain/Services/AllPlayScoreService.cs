using FbRider.Api.Domain.Models;

namespace FbRider.Api.Domain.Services;

public class AllPlayScoreService : IAllPlayScoreService
{
    public IEnumerable<AllPlayMatchUp> GetMatchUps(Dictionary<string, TeamStats> weeklyTeamStats)
    {
        List<AllPlayMatchUp> matchUps = new();
        foreach (var team1 in weeklyTeamStats)
        foreach (var team2 in weeklyTeamStats)
            if (team1.Key != team2.Key &&
                !matchUps.Any(m => m.Team1.TeamKey == team2.Key && m.Team2.TeamKey == team1.Key))
                matchUps.Add(new AllPlayMatchUp { Team1 = team1.Value, Team2 = team2.Value });

        return matchUps;
    }

    public Dictionary<string, AllPlayScore> GetWeeklyTeamScores(Dictionary<string, TeamStats> weeklyTeamStats)
    {
        var matchUps = GetMatchUps(weeklyTeamStats);
        Dictionary<string, AllPlayScore> teamScores = new();

        //initialize the dictionary
        foreach (var team in weeklyTeamStats.Keys)
        {
            var teamScore = new AllPlayScore() { TeamKey = team, StatScores = [] };
            teamScores[team] = teamScore;
            foreach (var statCategory in weeklyTeamStats.Values.First().Stats)
                teamScore.StatScores[statCategory.CategoryId] = 0;
        }
        //calculate the scores
        foreach (var matchUp in matchUps)
        {
            var team1 = matchUp.Team1.TeamKey;
            var team2 = matchUp.Team2.TeamKey;
            var team1Result = teamScores[team1];
            var team2Result = teamScores[team2];

            foreach (var category in matchUp.MatchUpResult.CategoriesTeam1Won) team1Result.StatScores[category]++;
            foreach (var category in matchUp.MatchUpResult.CategoriesTeam2Won) team2Result.StatScores[category]++;
            foreach (var category in matchUp.MatchUpResult.CategoriesTie)
            {
                team1Result.StatScores[category] += 0.5f;
                team2Result.StatScores[category] += 0.5f;
            }
        }

        return teamScores;
    }

    public Dictionary<string, AllPlayScore> GetSeasonTeamScores(
        Dictionary<int, Dictionary<string, TeamStats>> seasonTeamStatsByWeek)
    {
        Dictionary<string, AllPlayScore> result = new();
        //initialize the dictionary
        foreach (var teamStats in seasonTeamStatsByWeek.First().Value)
        {
            var seasonAllPlayScore = new AllPlayScore()
            {
                TeamKey = teamStats.Key,
                StatScores = new Dictionary<int, float>()
            };
            foreach (var stat in teamStats.Value.Stats) seasonAllPlayScore.StatScores[stat.CategoryId] = 0;
            result.Add(teamStats.Key, seasonAllPlayScore);
        }

        //sum up the scores
        foreach (var weeklyStats in seasonTeamStatsByWeek)
        {
            var weeklyTeamScores = GetWeeklyTeamScores(weeklyStats.Value);
            foreach (var teamScores in weeklyTeamScores)
            foreach (var stats in teamScores.Value.StatScores)
                result[teamScores.Key].StatScores[stats.Key] += stats.Value;
        }

        return result;
    }
}