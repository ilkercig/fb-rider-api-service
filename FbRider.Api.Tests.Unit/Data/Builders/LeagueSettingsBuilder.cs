using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbRider.Api.Domain;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders
{
    internal class SettingsBuilder
    {
        private Settings _settings;

        public SettingsBuilder()
        {
            _settings = new Settings()
            {
                StatCategories = new StatCategories()
                {
                    Stats =
                    [
                        new StatBuilder().WithOnlyDisplayCategory().Build(),
                        new StatBuilder().WithNegativeCategory().Build(),
                        new StatBuilder().WithPositiveCategory().Build()
                    ]
                },
                RosterPositions = 
                    [
                        new RosterPositionBuilder().WithStartingPosition().Build(),
                        new RosterPositionBuilder().WithBenchPosition().Build(),
                        new RosterPositionBuilder().WithInjuryPosition().Build()
                    ]
            };
        }

        public SettingsBuilder WithStatCategories(Stat[] statCategories)
        {
            _settings.StatCategories = new StatCategories()
            {
                Stats = statCategories
            };
            return this;
        }

        public SettingsBuilder WithRosterPositions(RosterPosition[] rosterPositions)
        {
            _settings.RosterPositions = rosterPositions;
            return this;
        }

        public Settings Build()
        {
            return _settings;
        }
    }
}
