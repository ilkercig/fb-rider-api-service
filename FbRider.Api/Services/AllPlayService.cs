using FbRider.Api.Domain.Models;
using FbRider.Api.DTOs;
using FbRider.Api.YahooApi;
using System.Globalization;
using FbRider.Api.Domain.Services;

namespace FbRider.Api.Services;

public class AllPlayService(IYahooFantasySportsApiClient apiClient, ILeagueService leagueService, IAllPlayScoreService allPlayScoreService) : IAllPlayService
{
    public async Task<AllPlayStandingsDTO> GetTeamAllPlaySeasonScores(string accessToken, string leagueKey)
    {
        var league = await leagueService.GetLeagueAsync(accessToken, leagueKey);
        if (league.Settings == null || league.Teams == null)
        {
            throw new InvalidOperationException($"Settings and teams are not found for the league:{leagueKey}");
        }

        Dictionary<int, Dictionary<string, TeamStats>> allWeeksAllPlays = new();
        foreach (var week in Enumerable.Range(league.StartWeek, league.CurrentWeek - league.StartWeek))
        {
            var weeklyAllPlay = await GetTeamStatsByWeek(accessToken, leagueKey, week);
            allWeeksAllPlays[week] = weeklyAllPlay;

        }
        var seasonAllPlay = allPlayScoreService.GetSeasonTeamScores(allWeeksAllPlays);
        var result = new AllPlayStandingsDTO()
        {
            StatCategories = league.Settings.StatCategories.Where(s => !s.IsOnlyDisplayStat),
            TeamScores = seasonAllPlay
                .Select(b => new TeamScoreDTO()
                {
                    TeamKey = b.Key,
                    StatScores = b.Value.StatScores,
                    TeamName = league.Teams.Single(t => t.Key == b.Key).Name
                }).ToList()
        };
        return result;
    }
    public async Task<Dictionary<string, TeamStats>> GetTeamStatsByWeek(string accessToken, string leagueKey, int week)
    {
        var league = await leagueService.GetLeagueAsync(accessToken, leagueKey);
        Dictionary<string, TeamStats> teamStatsDict = new();
        foreach (var team in league.Teams!)
        {
            var teamStats = await apiClient.GetTeamStatsByWeek(accessToken, team.Key, week);
            var statTuple = teamStats.Stats.Select(stat => (stat, category: league.Settings!.StatCategories
                .Single(l => l.Id.ToString() == stat.StatId)));
            teamStatsDict.Add(team.Key, new TeamStats
            {
                TeamKey = team.Key,
                Stats = statTuple.Where(t=>!t.category.IsOnlyDisplayStat).Select(t => CreateStat(t.category.Id, t.stat.Value, t.category.SortOrder)),
            });
        }
        return teamStatsDict;
    }
    private IComparableStat CreateStat(int statCategoryId, string? value, int sortOrder)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException("Stat value is required.");
        }
        if (sortOrder == 1)
            return new PositiveStat
            {
                CategoryId = statCategoryId,
                Value = float.Parse(value, CultureInfo.InvariantCulture)
            };
        else
            return new NegativeStat
            {
                CategoryId = statCategoryId,
                Value = float.Parse(value, CultureInfo.InvariantCulture)
            };
    }
}