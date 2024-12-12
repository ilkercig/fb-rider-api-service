using System.Net.Http.Headers;
using System.Xml.Serialization;
using FbRider.Api.DTOs.Resources;

namespace FbRider.Api.YahooApi
{
    public class YahooFantasySportsApiClient(HttpClient httpClient) : YahooApiClientBase(httpClient), IYahooFantasySportsApiClient
    {
        protected override YahooApiType ApiType => YahooApiType.FantasySports;

        public async Task<League> GetLeague(string accessToken, string leagueKey)
        {
            var result = await GetFantasyContentAsync(accessToken, YahooApiUrls.LeagueUrl, leagueKey,
                "settings,standings,scoreboard");
            if (result.League == null)
                throw new YahooApiException(YahooApiErrorMessages.LeagueNotFound, leagueKey, YahooApiType.FantasySports);
            return result.League;
        }

        public async Task<FantasyContent> GetFantasyContentAsync(string accessToken, string url, string resourceKey,
            string subResources)
        {
            url = $"{url}/{resourceKey};out={subResources}";
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
