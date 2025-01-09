using FbRider.Api.Domain.Models;
using FbRider.Api.Services;
using FbRider.Api.YahooApi;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbRider.Api.Domain.Services;
using FbRider.Api.Tests.Unit.Data.Builders;

namespace FbRider.Api.Tests.Unit.Services
{
    [TestFixture]
    public class AllPlayServiceTests
    {
        private Mock<IYahooFantasySportsApiClient> _apiClientMock;
        private Mock<ILeagueService> _leagueServiceMock;
        private AllPlayService _allPlayService;

        private const string AccessToken = "access_token";

        [SetUp]
        public void SetUp()
        {
            _apiClientMock = new Mock<IYahooFantasySportsApiClient>();
            _leagueServiceMock = new Mock<ILeagueService>();
            _allPlayService = new AllPlayService(_apiClientMock.Object, _leagueServiceMock.Object, new AllPlayScoreService());
        }

        [Test]
        public void GetWeeklyBeasts_LeagueStandingsNotFound_Throws()
        {
            var leagueData = new LeagueBuilder().Build();
            _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);
            //Assert.ThrowsAsync<InvalidOperationException>(() => _allPlayService.GetWeeklyAllPlay(AccessToken, leagueData.LeagueKey, 1));
        }


        [Test]
        public void GetWeeklyBeasts_ReturnsWeeklyBeasts()
        {
            int currentWeek = 3;
            Team[] teams =
            [
                new TeamBuilder().WithTeamKey("team_1").Build(),
                new TeamBuilder().WithTeamKey("team_2").Build(),
                new TeamBuilder().WithTeamKey("team_3").Build(),
                new TeamBuilder().WithTeamKey("team_4").Build()
            ];

            Dictionary<int, Dictionary<string, Api.YahooApi.TeamStatsResource>> weeklyStats = new()
            {
                {
                    1, new()
                    {
                        {"team_1", new TeamStatsBuilder().With3CatStats("550", ".460", "41", "12/43" ).Build()},
                        {"team_2", new TeamStatsBuilder().With3CatStats("702", ".482", "51", "21/34").Build()},
                        {"team_3", new TeamStatsBuilder().With3CatStats("456", ".401", "80", "21/34").Build()},
                        {"team_4", new TeamStatsBuilder().With3CatStats("980", ".571", "69", "21/34").Build()},

                    }
                },
                {
                    2, new()
                    {
                        {"team_1", new TeamStatsBuilder().With3CatStats("561", ".430", "65", "21/34").Build()},
                        {"team_2", new TeamStatsBuilder().With3CatStats("656", ".460", "23", "21/43").Build()},
                        {"team_3", new TeamStatsBuilder().With3CatStats("316", ".467", "23", "21/34").Build()},
                        {"team_4", new TeamStatsBuilder().With3CatStats("841", ".423", "65", "21/34").Build()},
                    }
                }
            };

            var leagueData = new LeagueBuilder().WithSettings(new SettingsBuilder().Build()).WithCurrentWeek(currentWeek.ToString()).WithTeams(teams).Build();

            _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);

            foreach (var week in Enumerable.Range(1, currentWeek - 1))
            {
                foreach (var team in teams)
                {
                    _apiClientMock.Setup(x => x.GetTeamStatsByWeek(AccessToken, team.TeamKey, week)).ReturnsAsync(weeklyStats[week][team.TeamKey]);
                }
            }

            var allPlayScores = _allPlayService.GetTeamAllPlaySeasonScores(AccessToken, leagueData.LeagueKey).Result;
            //Assert.That(weeklyBeasts.Count(), Is.EqualTo(2));
            //Assert.That(weeklyBeasts[1].TeamStats.Count(), Is.EqualTo(4));
            //Assert.That(weeklyBeasts[1].TeamStats["team_1"].Stats.Count(), Is.EqualTo(3));
            //Assert.That(weeklyBeasts[1].TeamStats["team_1"].Stats.Single(s => s.StatCategoryId == 19).Value, Is.EqualTo(41));
            //Assert.That(weeklyBeasts[1].TeamStats["team_1"].Stats.Single(s => s.StatCategoryId == 12).Value, Is.EqualTo(550));
            //Assert.That(weeklyBeasts[1].TeamStats["team_1"].Stats.Single(s => s.StatCategoryId == 5).Value, Is.EqualTo(0.46f));
            //Assert.That(weeklyBeasts[1].TeamStats["team_1"].Stats.Single(s => s.StatCategoryId == 19).GetType(), Is.EqualTo(typeof(NegativeStat)));
            //Assert.That(weeklyBeasts[1].TeamStats["team_1"].Stats.Single(s => s.StatCategoryId == 5).GetType(), Is.EqualTo(typeof(PositiveStat)));

        }
    }
}
