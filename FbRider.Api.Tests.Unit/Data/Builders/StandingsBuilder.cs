using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders;

public class StandingsBuilder
{
    private Standings _standings;

    public StandingsBuilder()
    {
        _standings = new Standings()
        {
            Teams = [
                new TeamBuilder().Build(),
                new TeamBuilder().WithTeamKey("a_team_key").Build(),
                new TeamBuilder().WithTeamKey("another_team_key").Build()
            ]

        };
    }

    public StandingsBuilder WithTeams(Team[] teams)
    {
        _standings.Teams = teams;
        return this;
    }

    public Standings Build()
    {
        return _standings;
    }
}