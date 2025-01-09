using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FbRider.Api.Services;
using FbRider.Api.YahooApi;
using Microsoft.AspNetCore.Authentication.BearerToken;
using NuGet.Frameworks;
using System.Xml.Linq;
using FbRider.Api.Utils;
using FbRider.Api.Domain.Models;
using FbRider.Api.Tests.Unit.Data.Builders;
using AutoMapper;
using FbRider.Api.Mapping;

namespace FbRider.Api.Tests.Unit.Services
{
    [TestFixture]
    public class LeagueServiceTests
    {
        private Mock<IYahooFantasySportsApiClient> _apiClientMock;
        private LeagueService _leagueService;
        private const string AccessToken = "access_token";
        private IMapper _mapper;



        [SetUp]
        public void SetUp()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<YahooApiResourceMappingProfile>()).CreateMapper();
            _apiClientMock = new Mock<IYahooFantasySportsApiClient>();
            _leagueService = new LeagueService(_apiClientMock.Object, _mapper);
        }

        [Test]
        public async Task GetUserSeasons_UserHasNoGame_ReturnsEmpty()
        {
            _apiClientMock.Setup(x => x.GetUserGames(AccessToken)).ReturnsAsync([]);
            var result = await _leagueService.GetUserSeasonsAsync(AccessToken);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetUserSeasons_UserHasNoNbaGame_ReturnsEmpty()
        {
            //Arrange
            var nflGame = new GameBuilder().WithCode("nfl").Build();

            _apiClientMock.Setup(x => x.GetUserGames(AccessToken)).ReturnsAsync([nflGame]);
            var result = await _leagueService.GetUserSeasonsAsync(AccessToken);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetUserSeasons_UserHasNbaGame_ReturnsSeasons()
        {
            //Arrange
            var nflGame = new GameBuilder().WithCode("nfl").Build();
            var activeNbaGame = new GameBuilder().WithCode("nba").WithIsGameOver("0").Build();
            var finishedNbaGame = new GameBuilder().WithCode("nba").WithIsGameOver("1").Build();

            _apiClientMock.Setup(x => x.GetUserGames(AccessToken)).ReturnsAsync([nflGame, activeNbaGame, finishedNbaGame]);
            var seasons = await _leagueService.GetUserSeasonsAsync(AccessToken);
            Assert.IsNotNull(seasons);
            Assert.IsNotEmpty(seasons);
            Assert.That(seasons.Length, Is.EqualTo(2));

            Assert.IsFalse(seasons[0].IsSeasonOver);
            Assert.That(seasons[0].Season.ToString(), Is.EqualTo(activeNbaGame.Season));
            Assert.That(seasons[0].Key, Is.EqualTo(activeNbaGame.GameKey));

        }

        [Test]
        public async Task GetUserSeasons_UserHasNbaGameButNoLeague_ReturnsSeasonWithEmptyLeague()
        {
            var nbaGameNoLeague = new GameBuilder().WithCode("nba").WithIsGameOver("0").WithLeagues([]).Build();

            _apiClientMock.Setup(x => x.GetUserGames(AccessToken)).ReturnsAsync([nbaGameNoLeague]);
            var seasons = await _leagueService.GetUserSeasonsAsync(AccessToken);

            Assert.IsNotNull(seasons.First().Leagues);
            Assert.IsEmpty(seasons.First().Leagues);
        }

        [Test]
        public async Task GetUserSeasons_UserHasNbaLeague_ReturnsSeasonWithLeague()
        {
            var leagueData = new LeagueBuilder().Build();
            var gameData = new GameBuilder().WithLeagues([leagueData]).Build();
            _apiClientMock.Setup(x => x.GetUserGames(AccessToken)).ReturnsAsync([gameData]);
            var seasons = await _leagueService.GetUserSeasonsAsync(AccessToken);

            Assert.IsNotNull(seasons.First().Leagues);
            Assert.IsNotEmpty(seasons.First().Leagues);

            //assert league
            var league = seasons.Single().Leagues.First();
            Assert.That(league.Key, Is.EqualTo(leagueData.LeagueKey)); 
            Assert.That(league.Name, Is.EqualTo(leagueData.Name));
            Assert.That(league.LogoUrl, Is.EqualTo(leagueData.LogoUrl));
            Assert.That(league.ScoringType, Is.EqualTo(EnumConvertor.GetScoringType(leagueData.ScoringType)));

        }

        [Test]
        public async Task GetUserActiveLeagues_UserHasNoActiveLeagues_ReturnsEmpty()
        {
            var gameData = new GameBuilder().WithIsGameOver("1").Build();
            _apiClientMock.Setup(x => x.GetUserGames(AccessToken)).ReturnsAsync([gameData]);
            var leagues = await _leagueService.GetUserActiveLeaguesAsync(AccessToken, ScoringType.HeadToHead);

            Assert.IsNotNull(leagues);
            Assert.IsEmpty(leagues);
        }

        [Test]
        public async Task GetUserActiveLeagues_UserHasActiveLeagues_ReturnLeagues()
        {
            var leagueData = new LeagueBuilder().Build();
            var leagueData2 = new LeagueBuilder().WithLeagueKey("league_key_2").Build();
            var activeGame = new GameBuilder().WithLeagues([leagueData, leagueData2]).Build();
            var finishedGame = new GameBuilder().WithIsGameOver("1").WithLeagues([leagueData]).Build();

            _apiClientMock.Setup(x => x.GetUserGames(AccessToken)).ReturnsAsync([activeGame, finishedGame]);
            var leagues = await _leagueService.GetUserActiveLeaguesAsync(AccessToken, ScoringType.HeadToHead);

            Assert.IsNotNull(leagues);
            Assert.That(leagues.Length, Is.EqualTo(activeGame.Leagues.Length));
            Assert.That(leagues.First().Key, Is.EqualTo(activeGame.Leagues.First().LeagueKey));
        }

        [Test]
        public async Task GetUserActiveLeagues_UserHasAvtiveLeaguesButNotRightScoringType_ReturnLeagues()
        {
            var rotoLeague = new LeagueBuilder().WithScoringType("roto").Build();
            var activeGame = new GameBuilder().WithLeagues([rotoLeague]).Build();

            _apiClientMock.Setup(x => x.GetUserGames(AccessToken)).ReturnsAsync([activeGame]);
            var leagues = await _leagueService.GetUserActiveLeaguesAsync(AccessToken, ScoringType.HeadToHead);

            Assert.IsNotNull(leagues);
            Assert.IsEmpty(leagues);
        }

        [Test]
        public void GetUserTeamByLeague_UserIsNotManager_ThrowsException()
        {
            var leagueData = new LeagueBuilder().WithStandings(new StandingsBuilder().WithTeams([new TeamBuilder().WithManagers([new ManagerBuilder().Build()]).Build()]).Build()).Build();
            _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);
            Assert.ThrowsAsync<ArgumentException>(() => _leagueService.GetUserTeamByLeagueAsync(AccessToken, leagueData.LeagueKey));
        }

        [Test]
        public void GetUserTeamByLeague_RosterNotFound_ThrowsException()
        {
            var leagueData = new LeagueBuilder().WithStandings(new StandingsBuilder().WithTeams([new TeamBuilder().WithOwnTeam().WithEmptyRoster().Build()]).Build()).Build();
            _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);
            _apiClientMock.Setup(x => x.GetTeam(AccessToken, leagueData.Standings.Teams.First().TeamKey)).ReturnsAsync(new TeamBuilder().WithEmptyRoster().Build());
            Assert.ThrowsAsync<InvalidOperationException>(() => _leagueService.GetUserTeamByLeagueAsync(AccessToken, leagueData.LeagueKey));
        }

        [Test]
        public void GetUserTeamByLeague_WithEmptyRoster_ReturnsTeamWithRoster()
        {
            var leagueData = new LeagueBuilder().WithTeams([new TeamBuilder().WithOwnTeam().Build()]).Build();
            var teamData = new TeamBuilder().WithRoster(new RosterBuilder().WithPlayers([]).Build()).Build();
            _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);
            _apiClientMock.Setup(x => x.GetTeam(AccessToken, leagueData.Standings.Teams.First().TeamKey)).ReturnsAsync(teamData);
            var team = _leagueService.GetUserTeamByLeagueAsync(AccessToken, leagueData.LeagueKey).Result;
            Assert.IsNotNull(team);
            Assert.That(team.Key, Is.EqualTo(teamData.TeamKey));
            Assert.That(team.Name, Is.EqualTo(teamData.Name));
            Assert.That(team.TeamLogo, Is.EqualTo(teamData.TeamLogos?.First()?.Url));
            Assert.IsNotNull(team.Roster);
            Assert.IsEmpty(team.Roster.Players);
        }

        [Test]
        public void GetUserTeamByLeague_WithRoster_ReturnsTeamWithRoster()
        {
            var userTeam = new TeamBuilder().WithOwnTeam().Build();
            var leagueData = new LeagueBuilder().WithTeams([userTeam]).Build();

            _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);
            _apiClientMock.Setup(x => x.GetTeam(AccessToken, leagueData.Standings.Teams.First().TeamKey)).ReturnsAsync(userTeam);
            var team = _leagueService.GetUserTeamByLeagueAsync(AccessToken, leagueData.LeagueKey).Result;
            Assert.IsNotNull(team);
            Assert.IsNotNull(team.Roster);
            Assert.IsNotEmpty(team.Roster.Players);
            AssertPlayer(userTeam.Roster.Players.First(), team.Roster.Players.First());
        }

        [Test]
        public async Task GetLeagueSettings_ReturnsStatCategories()
        {
            var settingsData = new SettingsBuilder().Build();
            var leagueData = new LeagueBuilder().WithSettings(settingsData).WithTeams([new TeamBuilder().Build()]).Build();
            _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);
            var settings = (await _leagueService.GetLeagueAsync(AccessToken, leagueData.LeagueKey)).Settings;
            Assert.IsNotNull(settings);
            Assert.IsNotEmpty(settings.StatCategories);
            Assert.That(settings.StatCategories.Count(), Is.EqualTo(3));
            Assert.That(settings.StatCategories.First().Id.ToString(), Is.EqualTo(settingsData.StatCategories.Stats.First().StatId));
            Assert.That(settings.StatCategories.First().Name, Is.EqualTo(settingsData.StatCategories.Stats.First().Name));
        }


        [Test]
        public void GetLeague_ReturnsTeams()
        {
            var leagueData = new LeagueBuilder().WithTeams([new TeamBuilder().Build()]).WithSettings(new SettingsBuilder().Build()).Build();
            _apiClientMock.Setup(x => x.GetLeague(AccessToken, leagueData.LeagueKey)).ReturnsAsync(leagueData);
            var league = _leagueService.GetLeagueAsync(AccessToken, leagueData.LeagueKey).Result;
            Assert.IsNotNull(league);
            Assert.IsNotEmpty(league.Teams);
            Assert.That(league.Teams.Count(), Is.EqualTo(1));
            Assert.That(league.Teams.First().Key, Is.EqualTo(leagueData.Standings.Teams.First().TeamKey));
        }


        private void AssertPlayer(Player playerApiData, FantasyPlayer player)
        {
            Assert.That(player.Key, Is.EqualTo(playerApiData.PlayerKey));
            Assert.That(player.FullName, Is.EqualTo(playerApiData.Name.Full));
            CollectionAssert.AreEqual(player.EligiblePositions, playerApiData.EligiblePositions);
            Assert.That(player.ImageUrl, Is.EqualTo(playerApiData.ImageUrl));
            Assert.That(player.SelectedPosition, Is.EqualTo(playerApiData.SelectedPosition?.Position));
        }
    }
}