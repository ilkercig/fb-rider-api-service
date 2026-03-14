using AutoMapper;
using FbRider.Domain.Models;
using FbRider.Application.Services;
using DomainLeague = FbRider.Domain.Models.League;
using DomainTeam = FbRider.Domain.Models.Team;

namespace FbRider.YahooApi;

public class LeagueService(IYahooFantasySportsApiClient apiClient, IMapper mapper) : ILeagueService
{
    public async Task<Season[]> GetUserSeasonsAsync(string accessToken)
    {
        var games = await apiClient.GetUserFantasyGames(accessToken);
        var nbaGames = games.Where(g => g.Code == "nba");
        return mapper.Map<Season[]>(nbaGames);
    }
    public async Task<DomainLeague> GetLeagueAsync(string accessToken, string leagueKey)
    {
        var leagueData = await apiClient.GetLeagueWithAllSubresources(accessToken, leagueKey);
        return mapper.Map<DomainLeague>(leagueData);
    }

    public async Task<DomainLeague[]> GetUserActiveLeaguesAsync(string accessToken, ScoringType scoringType)
    {
        var seasons = await GetUserSeasonsAsync(accessToken);
        return seasons.Where(s => !s.IsSeasonOver)
            .SelectMany(s => s.Leagues)
            .Where(l => l.ScoringType == scoringType)
            .ToArray();
    }
    public async Task<DomainTeam> GetUserTeamByLeagueAsync(string accessToken, string leagueKey)
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

        team.Roster = mapper.Map<TeamRoster>(teamWithRoster.Roster);

        return team;
    }

}
