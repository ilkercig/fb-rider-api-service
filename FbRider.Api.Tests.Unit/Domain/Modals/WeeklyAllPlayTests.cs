using FbRider.Api.Domain.Models;
using FbRider.Api.Domain.Services;

namespace FbRider.Api.Tests.Unit.Domain.Modals
{
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
            //var matchups = weeklyAllPlay.MatchUps.ToList();
            var matchups = service.GetMatchUps(teamStats).ToList();

            // Assert

            // a. No duplicated matchups
            var uniqueMatchups = new HashSet<(string Team1, string Team2)>();
                foreach (var matchup in matchups)
                {
                    var pair = matchup.Team1.TeamKey.CompareTo(matchup.Team2.TeamKey) < 0
                        ? (matchup.Team1.TeamKey, matchup.Team2.TeamKey)
                        : (matchup.Team2.TeamKey, matchup.Team1.TeamKey);
                    uniqueMatchups.Add(pair);
                }
                Assert.AreEqual(matchups.Count, uniqueMatchups.Count, "There are duplicate matchups.");

                // b. All expected matchups created
                Assert.AreEqual(expectedMatchupCount, matchups.Count, "Not all expected matchups were created.");

                // c. No self-matchups
                foreach (var matchup in matchups)
                {
                    Assert.AreNotEqual(matchup.Team1.TeamKey, matchup.Team2.TeamKey, "A team is matched against itself.");
                }
            }

        [Test]
        public void WeeklyAllPlay_CorrectTeamScores_WithTies()
        {
            // Arrange
            var teamStats = CreateTeamStats();
            //var weeklyAllPlay = new WeeklyAllPlay(teamStats);
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
                    Assert.AreEqual(expectedScores[teamKey][category], actualScore[category],
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
                {
                    1, CreateTeamStats()
                },
                {
                    2, CreateTeamStats()
                },
                {
                    3, CreateTeamStats()
                }
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
                    Assert.AreEqual(expectedScores[teamKey][category], actualScore[category],
                        $"Mismatch in score for Team {teamKey}, Category {category}. Expected: {expectedScores[teamKey][category]}, but was: {actualScore[category]}.");
                }
            }


        }

    }
}
