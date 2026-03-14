using FbRider.Domain.Models;
using FbRider.AllPlay.Domain.Models;

namespace FbRider.AllPlay.Tests.Domain.Models;

public class AllPlayMatchUpTests
{
    [Test]
    public void MatchUpResult_BasicComparison_CategorizesCorrectly()
    {
        // Arrange
        var team1Stats = new List<IComparableStat>
        {
            new PositiveStat { CategoryId = 1, Value = 10 },
            new PositiveStat { CategoryId = 2, Value = 20 },
            new PositiveStat { CategoryId = 3, Value = 15 }
        };
        var team2Stats = new List<IComparableStat>
        {
            new PositiveStat { CategoryId = 1, Value = 5 },
            new PositiveStat { CategoryId = 2, Value = 25 },
            new PositiveStat { CategoryId = 3, Value = 15 }
        };

        var team1 = new TeamStats { TeamKey = "Team1", Stats = team1Stats };
        var team2 = new TeamStats { TeamKey = "Team2", Stats = team2Stats };

        var matchUp = new AllPlayMatchUp { Team1 = team1, Team2 = team2 };

        // Act
        var result = matchUp.MatchUpResult;

        // Assert
        Assert.That(result.CategoriesTeam1Won, Is.EqualTo(new[] { 1 }));
        Assert.That(result.CategoriesTeam2Won, Is.EqualTo(new[] { 2 }));
        Assert.That(result.CategoriesTie, Is.EqualTo(new[] { 3 }));
    }

    [Test]
    public void MatchUpResult_EmptyStats_ReturnsEmptyLists()
    {
        // Arrange
        var team1 = new TeamStats { TeamKey = "Team1", Stats = Enumerable.Empty<IComparableStat>() };
        var team2 = new TeamStats { TeamKey = "Team2", Stats = Enumerable.Empty<IComparableStat>() };

        var matchUp = new AllPlayMatchUp { Team1 = team1, Team2 = team2 };

        // Act
        var result = matchUp.MatchUpResult;

        // Assert
        Assert.That(result.CategoriesTeam1Won, Is.Empty);
        Assert.That(result.CategoriesTeam2Won, Is.Empty);
        Assert.That(result.CategoriesTie, Is.Empty);
    }

    [Test]
    public void MatchUpResult_MismatchedCategories_ThrowsException()
    {
        // Arrange
        var team1Stats = new List<IComparableStat>
        {
            new PositiveStat { CategoryId = 1, Value = 10 }
        };
        var team2Stats = new List<IComparableStat>
        {
            new PositiveStat { CategoryId = 2, Value = 5 }
        };

        var team1 = new TeamStats { TeamKey = "Team1", Stats = team1Stats };
        var team2 = new TeamStats { TeamKey = "Team2", Stats = team2Stats };

        var matchUp = new AllPlayMatchUp { Team1 = team1, Team2 = team2 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _ = matchUp.MatchUpResult);
    }

    [Test]
    public void MatchUpResult_NegativeStats_ReversesComparison()
    {
        // Arrange
        var team1Stats = new List<IComparableStat>
        {
            new NegativeStat { CategoryId = 1, Value = 10 }
        };
        var team2Stats = new List<IComparableStat>
        {
            new NegativeStat { CategoryId = 1, Value = 5 }
        };

        var team1 = new TeamStats { TeamKey = "Team1", Stats = team1Stats };
        var team2 = new TeamStats { TeamKey = "Team2", Stats = team2Stats };

        var matchUp = new AllPlayMatchUp { Team1 = team1, Team2 = team2 };

        // Act
        var result = matchUp.MatchUpResult;

        // Assert
        Assert.That(result.CategoriesTeam2Won, Is.EqualTo(new[] { 1 }));
        Assert.That(result.CategoriesTeam1Won, Is.Empty);
        Assert.That(result.CategoriesTie, Is.Empty);
    }

    [Test]
    public void MatchUpResult_DisplayStats_IgnoredInLogic()
    {
        // Arrange
        var team1Stats = new List<IComparableStat>
        {
            new PositiveStat { CategoryId = 1, Value = 10 }
        };
        var team2Stats = new List<IComparableStat>
        {
            new PositiveStat { CategoryId = 1, Value = 5 }
        };

        var displayStats = new[]
        {
            new DisplayStatValue { CategoryId = 1, Value = "10 points" }
        };

        var team1 = new TeamStats { TeamKey = "Team1", Stats = team1Stats, DisplayStats = displayStats };
        var team2 = new TeamStats { TeamKey = "Team2", Stats = team2Stats, DisplayStats = displayStats };

        var matchUp = new AllPlayMatchUp { Team1 = team1, Team2 = team2 };

        // Act
        var result = matchUp.MatchUpResult;

        // Assert
        Assert.That(result.CategoriesTeam1Won, Is.EqualTo(new[] { 1 }));
        Assert.That(result.CategoriesTeam2Won, Is.Empty);
        Assert.That(result.CategoriesTie, Is.Empty);
    }
}
