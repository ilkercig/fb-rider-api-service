using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders
{
    internal class RosterPositionBuilder
    {
        private RosterPosition _rosterPosition;

        public RosterPositionBuilder()
        {
            _rosterPosition = new RosterPosition();
        }

        public RosterPositionBuilder WithStartingPosition()
        {
            _rosterPosition = new RosterPosition
            {
                Position = "G",
                Count = "3",
                IsStartingPosition = "1",
                PositionType = "P"
            };
            return this;
        }

        public RosterPositionBuilder WithBenchPosition()
        {
            _rosterPosition = new RosterPosition
            {
                Position = "BN",
                Count = "2",
                IsStartingPosition = "0",
            };
            return this;
        }

        public RosterPositionBuilder WithInjuryPosition()
        {
            _rosterPosition = new RosterPosition
            {
                Position = "IL",
                Count = "1",
                IsStartingPosition = "0",
            };
            return this;
        }

        public RosterPosition Build()
        {
            return _rosterPosition;
        }
    }
}
