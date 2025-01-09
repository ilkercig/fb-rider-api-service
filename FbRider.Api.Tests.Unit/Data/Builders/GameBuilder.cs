using FbRider.Api.YahooApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FbRider.Api.Tests.Unit.Data.Builders
{
    public class GameBuilder
    {
        private readonly Game _game;

        public GameBuilder()
        {
            _game = new Game
            {
                GameKey = "1",
                GameId = "1",
                Name = "Basketball",
                Code = "nba",
                Type = "Default",
                Url = "http://default.url",
                Season = "2025",
                IsGameOver = "0",
                Leagues = [new LeagueBuilder().Build()]
            };
        }

        public GameBuilder WithGameKey(string gameKey)
        {
            _game.GameKey = gameKey;
            return this;
        }

        public GameBuilder WithGameId(string gameId)
        {
            _game.GameId = gameId;
            return this;
        }

        public GameBuilder WithName(string name)
        {
            _game.Name = name;
            return this;
        }

        public GameBuilder WithCode(string code)
        {
            _game.Code = code;
            return this;
        }

        public GameBuilder WithType(string type)
        {
            _game.Type = type;
            return this;
        }

        public GameBuilder WithSeason(string season)
        {
            _game.Season = season;
            return this;
        }

        public GameBuilder WithIsGameOver(string isGameOver)
        {
            _game.IsGameOver = isGameOver;
            return this;
        }

        public GameBuilder WithLeagues(League[] leagues)
        {
            _game.Leagues = leagues;
            return this;
        }

        // Additional methods for other properties can be added as needed

        public Game Build()
        {
            return _game;
        }
    }

}
