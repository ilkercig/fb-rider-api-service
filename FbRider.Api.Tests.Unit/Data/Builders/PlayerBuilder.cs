using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders;

public class PlayerBuilder
{
    private Player _player;

    public static PlayerBuilder Guard => new PlayerBuilder().WithPlayerKey("key.1").WithPlayerId("1")
        .WithFullName("Guard Player").WithEligiblePositions(["G,PG,SG,Util"]);
    public static PlayerBuilder Forward => new PlayerBuilder().WithPlayerKey("key.2").WithPlayerId("2")
        .WithFullName("Forward Player").WithEligiblePositions(["F,SF,PF,Util"]);
    public static PlayerBuilder Center => new PlayerBuilder().WithPlayerKey("key.3").WithPlayerId("3")
        .WithFullName("Center Player").WithEligiblePositions(["C,Util"]);


    public PlayerBuilder()
    {
        _player = new Player()
        {
            PlayerKey = "key.0",
            PlayerId = "0",
            Name = new Name()
            {
                Full = "default player",
                First = "default",
                Last = "player",
                AsciiFirst = "default",
                AsciiLast = "player"
            },
            EligiblePositions = ["G,PG,SG"],
            ImageUrl = "http://image.com",
            IsUndroppable = "1",
            DisplayPosition = "PG,SG",
            EditorialTeamAbbr = "DAL"
        };
    }

    public PlayerBuilder WithPlayerKey(string playerKey)
    {
        _player.PlayerKey = playerKey;
        return this;
    }

    public PlayerBuilder WithPlayerId(string playerId)
    {
        _player.PlayerId = playerId;
        return this;
    }

    public PlayerBuilder WithEligiblePositions(string[] eligiblePositions)
    {
        _player.EligiblePositions = eligiblePositions;
        return this;
    }

    public PlayerBuilder WithFullName(string fullName)
    {

        _player.Name = new Name()
        {
            Full = fullName,
            First = fullName.Split(" ")[0],
            Last = fullName.Split(" ")[1],
            AsciiFirst = fullName.Split(" ")[0],
            AsciiLast = fullName.Split(" ")[1]
        };
        return this;
    }

    public PlayerBuilder WithImageUrl(string imageUrl)
    {
        _player.ImageUrl = imageUrl;
        return this;
    }

    public PlayerBuilder WithIsUndroppable(string isUndroppable)
    {
        _player.IsUndroppable = isUndroppable;
        return this;
    }

    public PlayerBuilder WithDisplayPosition(string displayPosition)
    {
        _player.DisplayPosition = displayPosition;
        return this;
    }

    public PlayerBuilder WithSelectedPosition(string position)
    {
        var selectedPosition = new SelectedPosition()
        {
            CoverageType = "date",
            Date = DateTime.Now,
            Position = position,
            IsFlex = "1"
        };
        _player.SelectedPosition = selectedPosition;
        return this;
    }

    public PlayerBuilder WithStatus(string status)
    {
        _player.Status = status;
        return this;
    }




    public Player Build()
    {
        return _player;
    }
}