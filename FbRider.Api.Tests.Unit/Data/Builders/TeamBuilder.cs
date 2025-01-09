using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders;

public class TeamBuilder
{
    private Team _team;

    public TeamBuilder()
    {
        _team = new Team()
        {
            TeamKey = "key.l.key.t.1",
            TeamId = "1",
            Name = "Team Name",
            Managers = [new ManagerBuilder().Build()],
            TeamLogos = [new TeamLogo()
            {
                Url = "https://example.com/logo.png",
                Size = "large"
            }],
            Roster = new RosterBuilder().Build(),
            NumberOfMoves = "32"
        };
    }

    public TeamBuilder WithOwnTeam()
    {
        _team.Managers = [new ManagerBuilder().WithLoggedInUser().Build()];
        return this;
    }

    public TeamBuilder WithTeamKey(string teamKey)
    {
        _team.TeamKey = teamKey;
        return this;
    }

    public TeamBuilder WithTeamId(string teamId)
    {
        _team.TeamId = teamId;
        return this;
    }
    public TeamBuilder WithName(string name)
    {
        _team.Name = name;
        return this;
    }
    public TeamBuilder WithManagers(Manager[] managers)
    {
        _team.Managers = managers;
        return this;
    }

    public Team Build()
    {
        return _team;
    }

    public TeamBuilder WithRoster(Roster roster)
    {
        _team.Roster = roster;
        return this;
    }

    public TeamBuilder WithEmptyRoster()
    {
        _team.Roster = new Roster();
        return this;
    }

    public TeamBuilder WithTeamLogo(TeamLogo teamLogo)
    {
        _team.TeamLogos = [teamLogo];
        return this;
    }

    public TeamBuilder WithNoRoster()
    {
        _team.Roster = null;
        return this;
    }


    public TeamBuilder WithTeamStats(TeamStatsResource teamStatsResource)
    {
        _team.TeamStats = teamStatsResource;
        return this;
    }


}