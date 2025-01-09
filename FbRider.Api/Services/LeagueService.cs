using System.Globalization;
using AutoMapper;
using FbRider.Api.Domain.Models;
using FbRider.Api.Utils;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Services;

public class LeagueService(IYahooFantasySportsApiClient apiClient, IMapper mapper) : ILeagueService
{
    public async Task<FantasySeason[]> GetUserSeasonsAsync(string accessToken)
    {
        var games = await apiClient.GetUserGames(accessToken);
        var nbaGames =games.Where(g => g.Code == "nba");
        return mapper.Map<FantasySeason[]>(nbaGames);
    }
    public async Task<FantasyLeague> GetLeagueAsync(string accessToken, string leagueKey)
    {
        var leagueData = await apiClient.GetLeague(accessToken, leagueKey);
        return mapper.Map<FantasyLeague>(leagueData);
    }

    public async Task<FantasyLeague[]> GetUserActiveLeaguesAsync(string accessToken, ScoringType scoringType)
    {
        var seasons = await GetUserSeasonsAsync(accessToken);
        return seasons.Where(s => !s.IsSeasonOver)
            .SelectMany(s => s.Leagues)
            .Where(l => l.ScoringType == scoringType)
            .ToArray();
    }
    public async Task<FantasyTeam> GetUserTeamByLeagueAsync(string accessToken, string leagueKey)
    {
        var league = await GetLeagueAsync(accessToken, leagueKey);
        var team = league.Teams!.SingleOrDefault(t => t.Managers.Any(m => m.IsCurrentLogin));
        if (team == null)
        {
            throw new ArgumentException("The logged in user has no team in the league");
        }
        var teamWithRoster = await apiClient.GetTeam(accessToken, team.Key);
        if (teamWithRoster.Roster?.Players == null)
        {
            throw new InvalidOperationException("Roster not found for team");
        }

        team.Roster = mapper.Map<FantasyTeamRoster>(teamWithRoster.Roster);
        
        return team;
    }

}