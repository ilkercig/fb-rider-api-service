using FbRider.Domain.Models;
using FbRider.AllPlay.Domain.Services;

namespace FbRider.AllPlay.Tests.Domain.Models;

public class WeeklyAllPlayTests
{
    private Dictionary<string, TeamStats> CreateTeamStats()
    {
        return new Dictionary<string, TeamStats>
        {
            {
                "Team1", new TeamStats
                {
                    TeamKey = "Team1",
                    Stats = new List<IComparableStat>
                    {
                        new PositiveStat { CategoryId = 1, Value = 10 },
                        new PositiveStat { CategoryId = 2, Value = 20 },
                        new PositiveStat { CategoryId = 3, Value = 30 }
                    }
                }
            },
            {
                "Team2", new TeamStats
                {
                    TeamKey = "Team2",
                    Stats = new List<IComparableStat>
                    {
                        new PositiveStat { CategoryId = 1, Value = 15 },
                        new PositiveStat { CategoryId = 2, Value = 25 },
                        new PositiveStat { CategoryId = 3, Value = 30 }
                    }
                }
            },
            {
                "Team3", new TeamStats
                {
                    TeamKey = "Team3",
                    Stats = new List<IComparableStat>
                    {
                        new PositiveStat { CategoryId = 1, Value = 12 },
                        new PositiveStat { CategoryId = 2, Value = 22 },
                        new PositiveStat { CategoryId = 3, Value = 32 }
                    }
                }
            },
            {
                "Team4", new TeamStats
                {
                    TeamKey = "Team4",
                    Stats = new List<IComparableStat>
                    {
                        new PositiveStat { CategoryId = 1, Value = 18 },
                        new PositiveStat { CategoryId = 2, Value = 28 },
                        new PositiveStat { CategoryId = 3, Value = 38 }
                    }
                }
            }
        };
    }

    [Test]
    public void WeeklyAllPlay_NoDuplicateMatchups_AllExpectedMatchupsCreated_NoSelfMatchups()
    {
        // Arrange
        var teamStats = CreateTeamStats();
        AllPlayScoreService service = new AllPlayScoreService();
        var expectedMatchupCount = teamStats.Count * (teamStats.Count - 1) / 2; // Combination formula nC2

        // Act
        var matchups = service.GetMatchUps(teamStats).ToList();

        // Assert
        var uniqueMatchups = new HashSet<(string Team1, string Team2)>();
        foreach (var matchup in matchups)
        {
            var pair = matchup.Team1.TeamKey.CompareTo(matchup.Team2.TeamKey) < 0
                ? (matchup.Team1.TeamKey, matchup.Team2.TeamKey)
                : (matchup.Team2.TeamKey, matchup.Team1.TeamKey);
            uniqueMatchups.Add(pair);
        }
        Assert.That(matchups, Has.Count.EqualTo(uniqueMatchups.Count), "There are duplicate matchups.");
        Assert.That(matchups, Has.Count.EqualTo(expectedMatchupCount), "Not all expected matchups were created.");

        foreach (var matchup in matchups)
        {
            Assert.That(matchup.Team1.TeamKey, Is.Not.EqualTo(matchup.Team2.TeamKey), "A team is matched against itself.");
        }
    }

    [Test]
    public void WeeklyAllPlay_CorrectTeamScores_WithTies()
    {
        // Arrange
        var teamStats = CreateTeamStats();
        AllPlayScoreService service = new AllPlayScoreService();

        // Act
        var teamScores = service.GetWeeklyTeamScores(teamStats);

        // Assert
        var expectedScores = new Dictionary<string, Dictionary<int, float>>
        {
            { "Team1", new Dictionary<int, float> { { 1, 0 }, { 2, 0 }, { 3, 0.5f } } },
            { "Team2", new Dictionary<int, float> { { 1, 2f }, { 2, 2f }, { 3, 0.5f } } },
            { "Team3", new Dictionary<int, float> { { 1, 1f }, { 2, 1f }, { 3, 2f } } },
            { "Team4", new Dictionary<int, float> { { 1, 3f }, { 2, 3f }, { 3, 3f } } }
        };

        foreach (var teamKey in expectedScores.Keys)
        {
            var actualScore = teamScores[teamKey].StatScores;
            foreach (var category in expectedScores[teamKey].Keys)
            {
                Assert.That(actualScore[category], Is.EqualTo(expectedScores[teamKey][category]),
                    $"Mismatch in score for Team {teamKey}, Category {category}. Expected: {expectedScores[teamKey][category]}, but was: {actualScore[category]}.");
            }
        }
    }

    [Test]
    public void AllPlaySeasonScores_CorrectScores()
    {
        AllPlayScoreService service = new AllPlayScoreService();
        var seasonStats = new Dictionary<int, Dictionary<string, TeamStats>>
        {
            { 1, CreateTeamStats() },
            { 2, CreateTeamStats() },
            { 3, CreateTeamStats() }
        };
        var result = service.GetSeasonTeamScores(seasonStats);

        // Assert
        var expectedScores = new Dictionary<string, Dictionary<int, float>>
        {
            { "Team1", new Dictionary<int, float> { { 1, 0 }, { 2, 0 }, { 3, 1.5f } } },
            { "Team2", new Dictionary<int, float> { { 1, 6f }, { 2, 6f }, { 3, 1.5f } } },
            { "Team3", new Dictionary<int, float> { { 1, 3f }, { 2, 3f }, { 3, 6f } } },
            { "Team4", new Dictionary<int, float> { { 1, 9f }, { 2, 9f }, { 3, 9f } } }
        };

        foreach (var teamKey in expectedScores.Keys)
        {
            var actualScore = result[teamKey].StatScores;
            foreach (var category in expectedScores[teamKey].Keys)
            {
                Assert.That(actualScore[category], Is.EqualTo(expectedScores[teamKey][category]),
                    $"Mismatch in score for Team {teamKey}, Category {category}. Expected: {expectedScores[teamKey][category]}, but was: {actualScore[category]}.");
            }
        }
    }
}
