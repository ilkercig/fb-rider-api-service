using FbRider.Api.YahooApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FbRider.Api.Tests.Unit.Data.Builders
{
    public class LeagueBuilder
    {
        private readonly League _league;

        public LeagueBuilder()
        {
            _league = new League
            {
                LeagueKey = "1",
                LeagueId = "1",
                Name = "Default League",
                ScoringType = "head",
                Season = "2025",
                CurrentWeek = "7",
                StartWeek = "1",
                Url = "https://basketball.fantasysports.yahoo.com/f1/1",
                Standings = new StandingsBuilder().Build(),
                Settings = new SettingsBuilder().Build(),
            };

        }

        public LeagueBuilder WithLeagueKey(string leagueKey)
        {
            _league.LeagueKey = leagueKey;
            return this;
        }

        public LeagueBuilder WithName(string name)
        {
            _league.Name = name;
            return this;
        }

        public LeagueBuilder WithScoringType(string scoringType)
        {
            _league.ScoringType = scoringType;
            return this;
        }

        public LeagueBuilder WithSeason(string season)
        {
            _league.Season = season;
            return this;
        }

        public LeagueBuilder WithUrl(string? url)
        {
            _league.Url = url;
            return this;
        }

        // Additional methods for other properties can be added as needed

        public League Build()
        {
            return _league;
        }

        public LeagueBuilder WithStandings(Standings standings)
        {
            _league.Standings = standings;
            return this;
        }

        public LeagueBuilder WithTeams(Team[] teams)
        {
            var standings = new StandingsBuilder().WithTeams(teams).Build();
            _league.Standings = standings;
            return this;
        }

        public LeagueBuilder WithSettings(Settings settings)
        {
            _league.Settings = settings;
            return this;
        }

        public LeagueBuilder WithCurrentWeek(string currentWeek)
        {
            _league.CurrentWeek = currentWeek;
            return this;
        }

        public LeagueBuilder WithNoStandings()
        {
            _league.Standings = null;
            return this;
        }
        public LeagueBuilder WithNoSettings()
        {
            _league.Settings = null;
            return this;
        }

    }

}
