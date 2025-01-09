using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders;

public class RosterBuilder
{
    private Roster _roster;

    public RosterBuilder()
    {
        _roster = new Roster()
        {
            Players = [
                new PlayerBuilder().Build(),
                PlayerBuilder.Center.Build(),
                PlayerBuilder.Forward.Build(),
                PlayerBuilder.Guard.Build()
            ],
            IsEditable = "1"
        };
    }

    public Roster Build()
    {
        return _roster;
    }

    public RosterBuilder WithPlayers(Player[] players)
    {
        _roster.Players = players;
        return this;
    }

}