using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FbRider.Api.Domain.Models;
using FbRider.Api.Mapping;
using FbRider.Api.Tests.Unit.Data.Builders;
using FbRider.Api.Utils;
using Manager = FbRider.Api.YahooApi.Manager;
using RosterPosition = FbRider.Api.YahooApi.RosterPosition;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Mappings
{
    [TestFixture]
    public class YahooApiDataMappingProfileTests
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<YahooApiResourceMappingProfile>()).CreateMapper();
        }

        [Test]
        public void MapLeagueDataToFantasyLeague_LeagueKeyMapped()
        {
            var leagueData = new LeagueBuilder().Build();
            var league = _mapper.Map<FantasyLeague>(leagueData);

            //Assert
            Assert.AreEqual(leagueData.LeagueKey, league.Key);
        }

        [Test]
        [TestCaseSource(nameof(StatCategories))]
        public void MapStatDataToStatCategory_AllPropertiesValid(Stat statData)
        {
            //Act
            var statCategory = _mapper.Map<StatCategory>(statData);

            //Assert
            Assert.That(statCategory.Id.ToString(), Is.EqualTo(statData.StatId));
            Assert.That(statCategory.IsOnlyDisplayStat, Is.EqualTo(statData.IsOnlyDisplayStat == "1"));
            Assert.That(statCategory.Name, Is.EqualTo(statData.Name));
            Assert.That(statCategory.DisplayName, Is.EqualTo(statData.DisplayName));
            Assert.That(statCategory.SortOrder.ToString(), Is.EqualTo(statData.SortOrder));
            Assert.That(statCategory.Abbreviation, Is.EqualTo(statData.Abbr));
        }

        [Test]
        [TestCaseSource(nameof(RosterPositions))]
        public void MapRosterPositionDataToRosterPosition_AllPropertiesValid(RosterPosition rosterPositionData)
        {
            //Act
            var rosterPosition = _mapper.Map<FbRider.Api.Domain.Models.RosterPosition>(rosterPositionData);

            //Assert
            Assert.That(rosterPosition.Position, Is.EqualTo(rosterPositionData.Position));
            Assert.That(rosterPosition.Count.ToString(), Is.EqualTo(rosterPositionData.Count));
            Assert.That(rosterPosition.IsStartingPosition, Is.EqualTo(rosterPositionData.IsStartingPosition == "1"));

        }

        [Test]
        public void MapSettingsDataToLeagueSettings_StatCategoriesMapped()
        {
            var settingsData = new SettingsBuilder().Build();
            //Act
            var leagueSettings = _mapper.Map<LeagueSettings>(settingsData);
            //Assert
            Assert.That(leagueSettings.StatCategories.Count, Is.EqualTo(settingsData.StatCategories.Stats.Length));
            Assert.That(leagueSettings.RosterPositions.Count(), Is.EqualTo(settingsData.RosterPositions.Length));
        }

        [Test]
        [TestCaseSource(nameof(Players))] 
        public void MapPlayerDataToFantasyPlayer_AllPropertiesValid(Player playerData)
        {
            //Act
            var player = _mapper.Map<FantasyPlayer>(playerData);

            //Assert
            Assert.That(player.Key, Is.EqualTo(playerData.PlayerKey));
            Assert.That(player.Id, Is.EqualTo(playerData.PlayerId));
            Assert.That(player.FullName, Is.EqualTo(playerData.Name.Full));
            Assert.That(player.DisplayPositions, Is.EqualTo(playerData.DisplayPosition.Split(',')));
            CollectionAssert.AreEqual(player.EligiblePositions, playerData.EligiblePositions);
            Assert.That(player.IsUndroppable, Is.EqualTo(playerData.IsUndroppable == "1"));
            Assert.That(player.Status, Is.EqualTo(playerData.Status));
            Assert.That(player.SelectedPosition, Is.EqualTo(playerData.SelectedPosition?.Position));
        }

        [Test]
        public void MapRosterDataToFantasyTeamRoster_AllPropertiesValid()
        {
            var rosterData = new RosterBuilder().Build();
            //Act
            var roster = _mapper.Map<FantasyTeamRoster>(rosterData);
            //Assert
            Assert.That(roster.Players.Count, Is.EqualTo(rosterData.Players.Length));
        }

        [Test]
        public void MapManagerDataToManager_AllPropertiesValid()
        {
            var managerData = new ManagerBuilder().Build();
            //Act
            var manager = _mapper.Map<FbRider.Api.Domain.Models.Manager>(managerData);
            //Assert
            Assert.That(manager.Name, Is.EqualTo(managerData.Nickname));
            Assert.That(manager.Id.ToString(), Is.EqualTo(managerData.ManagerId));
            Assert.That(manager.IsCommissioner, Is.EqualTo(managerData.IsCommissioner == "1"));
            Assert.That(manager.IsCurrentLogin, Is.EqualTo(managerData.IsCurrentLogin == "1"));
        }

        [Test]
        [TestCaseSource(nameof(Teams))]
        public void MapTeamDataToFantasyTeam_AllPropertiesValid(Team teamData)
        {
            //Act
            var team = _mapper.Map<FantasyTeam>(teamData);
            //Assert
            Assert.That(team.Key, Is.EqualTo(teamData.TeamKey));
            Assert.That(team.Name, Is.EqualTo(teamData.Name));
            Assert.That(team.Id, Is.EqualTo(teamData.TeamId));
            Assert.That(team.TeamLogo, Is.EqualTo(teamData.TeamLogos?.FirstOrDefault()?.Url));
            Assert.That(team.Managers.Count(), Is.EqualTo(teamData.Managers.Length));
            if (team.Roster != null)
                Assert.That(team.Roster.Players.Count, Is.EqualTo(teamData.Roster.Players.Length));
            else
                Assert.That(teamData.Roster, Is.Null);
        }

        [Test]
        [TestCaseSource(nameof(Leagues))]
        public void MapLeagueDataToFantasyLeague_AllPropertiesValid(League leagueData)
        {
            //Act
            var league = _mapper.Map<FantasyLeague>(leagueData);
            //Assert
            Assert.That(league.Key, Is.EqualTo(leagueData.LeagueKey));
            Assert.That(league.Id, Is.EqualTo(leagueData.LeagueId));
            Assert.That(league.CurrentWeek, Is.EqualTo(Convert.ToInt32(leagueData.CurrentWeek)));
            Assert.That(league.LogoUrl, Is.EqualTo(leagueData.LogoUrl));
            Assert.That(league.ScoringType, Is.EqualTo(EnumConvertor.GetScoringType(leagueData.ScoringType)));
            Assert.That(league.StartWeek, Is.EqualTo(Convert.ToInt32(leagueData.StartWeek)));
            if (league.Settings == null || league.Teams == null)
            {
                Assert.That(leagueData.Settings, Is.Null);
                Assert.That(leagueData.Standings, Is.Null);
            }
            else
            {
                Assert.That(league.Teams.Count(), Is.EqualTo(leagueData.Standings?.Teams.Length ?? 0));
                Assert.That(league.Settings.StatCategories.Count(), Is.EqualTo(leagueData.Settings.StatCategories.Stats.Length));
            }
        }

        [Test]
        public void MapGameToFantasySeason_AllPropertiesValid()
        {
            var game = new GameBuilder().Build();

            //Act
            var season = _mapper.Map<FantasySeason>(game);

            //Assert
            Assert.That(season.Key, Is.EqualTo(game.GameKey));
            Assert.That(season.Season.ToString(), Is.EqualTo(game.Season));
            Assert.That(season.IsSeasonOver, Is.EqualTo(game.IsGameOver == "1"));
            Assert.That(season.Leagues.Count(), Is.EqualTo(game.Leagues.Length));
        }

        [Test]
        public void MapUserDataToFantasyUser_AllPropertiesValid()
        {
            var user = new UserBuilder().Build();
            //Act
            var fantasyUser = _mapper.Map<FantasyUser>(user);
            //Assert
            Assert.That(fantasyUser.Key, Is.EqualTo(user.Guid));
            Assert.That(fantasyUser.Seasons.Count(), Is.EqualTo(user.Games.Length));
        }


        //test case source
        public static IEnumerable<Stat> StatCategories()
        {
            yield return new StatBuilder().WithNegativeCategory().Build();
            yield return new StatBuilder().WithPositiveCategory().Build();
            yield return new StatBuilder().WithOnlyDisplayCategory().Build();
        }

        public static IEnumerable<RosterPosition> RosterPositions()
        {
            yield return new RosterPositionBuilder().WithStartingPosition().Build();
            yield return new RosterPositionBuilder().WithBenchPosition().Build();
            yield return new RosterPositionBuilder().WithInjuryPosition().Build();
        }

        public static IEnumerable<Player> Players()
        {
            yield return new PlayerBuilder().Build();
            yield return new PlayerBuilder().WithStatus("INJ").Build();
            yield return new PlayerBuilder().WithSelectedPosition("G").Build();
        }

        public static IEnumerable<Manager> Managers()
        {
            yield return new ManagerBuilder().Build();
            yield return new ManagerBuilder().WithCommissioner().Build();
            yield return new ManagerBuilder().WithLoggedInUser().Build();
        }

        public static IEnumerable<Team> Teams()
        {
            yield return new TeamBuilder().Build();
            yield return new TeamBuilder().WithNoRoster().Build();
        }

        public static IEnumerable<League> Leagues()
        {
            yield return new LeagueBuilder().Build();
            yield return new LeagueBuilder().WithNoSettings().WithNoStandings().Build();
        }

    }

    

}
