using System;
using System.Net;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using Polly;
using Polly.Retry;

namespace FbRider.YahooApi
{
    public class YahooFantasySportsApiClient(HttpClient httpClient) : YahooApiClientBase<YahooFantasySportsException>(httpClient, ResiliencePipeline), IYahooFantasySportsApiClient
    {
        private static readonly ResiliencePipeline<HttpResponseMessage> ResiliencePipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(response => response.StatusCode is HttpStatusCode.InternalServerError
                                              or HttpStatusCode.BadGateway
                                              or HttpStatusCode.ServiceUnavailable
                                              or HttpStatusCode.GatewayTimeout
                                              or HttpStatusCode.RequestTimeout),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1)
            })
            .Build();

        protected override YahooFantasySportsException CreateException(string message, string endpoint, string responseContent, HttpStatusCode statusCode, Exception? innerException = null)
        {
            return new YahooFantasySportsException(message, endpoint, responseContent, statusCode, innerException);
        }

        public async Task<League> GetLeagueWithAllSubresources(string accessToken, string leagueKey)
        {
            string url = $"{YahooApiUrls.LeagueUrl}/{leagueKey};out=settings,standings,scoreboard";
            var result = await GetFantasyContentAsync(accessToken, url);
            if (result.League?.Settings == null || result.League.Standings == null || result.League.Scoreboard == null)
                throw new YahooApiValidationException(YahooApiErrorMessages.LeagueNotFound, leagueKey);
            return result.League;
        }

        public async Task<Game[]> GetUserFantasyGames(string accessToken)
        {
            var result = await GetFantasyContentAsync(accessToken, YahooApiUrls.UserGamesUrl);
            if (result.Users == null || result.Users.Length == 0)
                throw new YahooApiValidationException(YahooApiErrorMessages.FantasyUserNotFound, YahooApiUrls.UserGamesUrl);
            
            return result.Users.First().Games ?? [];
        }

        public async Task<Team> GetTeam(string accessToken, string teamKey)
        {
            string url = $"{YahooApiUrls.TeamUrl}/{teamKey}/roster/players";
            var result = await GetFantasyContentAsync(accessToken, url);
            if (result.Team == null)
                throw new YahooApiValidationException(YahooApiErrorMessages.TeamNotFound, teamKey);

            return result.Team;
        }

        public async Task<TeamStatsResource> GetTeamStatsByWeek(string accessToken, string teamKey, int week)
        {
            string url = $"{YahooApiUrls.TeamUrl}/{teamKey}/stats;type=week;week={week}";
            var result = await GetFantasyContentAsync(accessToken, url);
            if (result.Team == null)
                throw new YahooApiValidationException(YahooApiErrorMessages.TeamNotFound, teamKey);
            if (result.Team.TeamStats == null)
                throw new YahooApiValidationException(YahooApiErrorMessages.TeamStatsNotFound, teamKey);
            return result.Team.TeamStats;
        }


        private async Task<FantasyContent> GetFantasyContentAsync(string accessToken, string url)
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
