using System;
using System.Net.Http.Headers;
using System.Xml.Serialization;

namespace FbRider.Api.YahooApi
{
    public class YahooFantasySportsApiClient(HttpClient httpClient) : YahooApiClientBase(httpClient), IYahooFantasySportsApiClient
    {
        protected override YahooApiType ApiType => YahooApiType.FantasySports;


        public async Task<League> GetLeague(string accessToken, string leagueKey)
        {
            string url = $"{YahooApiUrls.LeagueUrl}/{leagueKey};out=settings,standings,scoreboard";
            var result = await GetFantasyContentAsync(accessToken, url);
            if (result.League == null)
                throw new YahooApiException(YahooApiErrorMessages.LeagueNotFound, leagueKey, YahooApiType.FantasySports);
            return result.League;
        }

        public async Task<Game[]> GetUserGames(string accessToken)
        {
            var result = await GetFantasyContentAsync(accessToken, YahooApiUrls.UserGamesUrl);
            if (result.Users == null)
                throw new YahooApiException(YahooApiErrorMessages.FantasyUserNotFound, YahooApiUrls.UserGamesUrl, YahooApiType.FantasySports);
            if (result.Users.Single().Games == null) 
                throw new YahooApiException(YahooApiErrorMessages.FantasyUserGamesNotFound, YahooApiUrls.UserGamesUrl, YahooApiType.FantasySports);
            return result.Users.Single().Games;
        }

        public async Task<Team> GetTeam(string accessToken, string teamKey)
        {
            string url = $"{YahooApiUrls.TeamUrl}/{teamKey}/roster/players";
            var result = await GetFantasyContentAsync(accessToken, url);
            if (result.Team == null)
                throw new YahooApiException(YahooApiErrorMessages.TeamNotFound, teamKey, YahooApiType.FantasySports);

            return result.Team;
        }

        public async Task<TeamStatsResource> GetTeamStatsByWeek(string accessToken, string teamKey, int week)
        {
            string url = $"{YahooApiUrls.TeamUrl}/{teamKey}/stats;type=week;week={week}";
            var result = await GetFantasyContentAsync(accessToken, url);
            if (result.Team == null)
                throw new YahooApiException(YahooApiErrorMessages.TeamNotFound, teamKey, YahooApiType.FantasySports);
            if (result.Team.TeamStats == null)
                throw new YahooApiException(YahooApiErrorMessages.TeamStatsNotFound, teamKey, YahooApiType.FantasySports);
            return result.Team.TeamStats;
        }


        public async Task<FantasyContent> GetFantasyContentAsync(string accessToken, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var result = await base.SendRequest<FantasyContent>(request);
            return result;
        }

        protected override T? Deserialize<T>(string responseContent) where T : default
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(responseContent);
            return (T?)serializer.Deserialize(reader);
        }
    }
}
