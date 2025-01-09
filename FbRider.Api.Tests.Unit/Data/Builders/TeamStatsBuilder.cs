using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbRider.Api.YahooApi;

namespace FbRider.Api.Tests.Unit.Data.Builders
{
    internal class TeamStatsBuilder
    {
        public TeamStatsResource TeamStatsResource;

        public TeamStatsBuilder()
        {
            TeamStatsResource = new TeamStatsResource()
            {
                CoverageType = "week",
                Week = "1",
                Date = DateTime.Now,
                Season = "2021",
                Stats = []
            };
        }

        public TeamStatsBuilder WithCoverageType(string coverageType)
        {
            TeamStatsResource.CoverageType = coverageType;
            return this;
        }

        public TeamStatsBuilder WithWeek(string week)
        {
            TeamStatsResource.Week = week;
            return this;
        }

        public TeamStatsBuilder WithDate(DateTime date)
        {
            TeamStatsResource.Date = date;
            return this;
        }

        public TeamStatsBuilder WithSeason(string season)
        {
            TeamStatsResource.Season = season;
            return this;
        }

        public TeamStatsBuilder With3CatStats(string pts, string fg, string to, string fgaAndFgm)
        {
            var ptsStat = new Stat()
            {
                StatId = "12",
                Value = pts
            };
            var fgStat = new Stat()
            {
                StatId = "5",
                Value = fg
            };
            var toStat = new Stat()
            {
                StatId = "19",
                Value = to
            };
            var fgmAndFga = new Stat()
            {
                StatId = "9004003",
                Value = fgaAndFgm
            };
            TeamStatsResource.Stats = [ptsStat, fgStat, toStat];
            return this;
        }

        public TeamStatsResource Build()
        {
            return TeamStatsResource;
        }

    }
}
