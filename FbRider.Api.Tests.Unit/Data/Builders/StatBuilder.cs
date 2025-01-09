using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders
{
    internal class StatBuilder
    {
        private Stat _stat;

        public StatBuilder()
        {
        }

        public StatBuilder WithPositiveCategory()
        {
            _stat = new Stat()
            {
                StatId = "12",
                Enabled = "1",
                Name = "Points Scored",
                DisplayName = "PTS",
                Abbr = "PTS",
                SortOrder = "1",
            };
            return this;
        }

        public StatBuilder WithNegativeCategory()
        {
            _stat = new Stat()
            {
                StatId = "10",
                Enabled = "1",
                Name = "Turnovers",
                DisplayName = "TO",
                Abbr = "TO",
                SortOrder = "0",
            };
            return this;
        }

        public StatBuilder WithOnlyDisplayCategory()
        {
            _stat = new Stat()
            {
                StatId = "9007006",
                Enabled = "1",
                Name = "Free Throws Made / Free Throws Attempted",
                DisplayName = "FTM/A",
                Abbr = "FTM / FTA",
                SortOrder = "1",
                IsOnlyDisplayStat = "1",
                Group = "free_throws"
            };
            return this;
        }

        public Stat Build()
        {
            return _stat;
        }
    }
}
