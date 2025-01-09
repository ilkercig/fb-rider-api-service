namespace FbRider.Api.YahooApi
{
    public static class YahooApiErrorMessages
    {
        public const string ApiCallFailed = "The Yahoo API call returned an unexpected response.";
        public const string ApiRequestError = "Error occurred while making a request to the Yahoo API.";
        public const string DeserializationFailed = "Failed to deserialize the Yahoo API response.";
        public const string ResponseNotSuccessful = "Yahoo API returned an unsuccessful response.";
        public static string LeagueNotFound = "The league was not found.";
        public static string FantasyUserNotFound = "The fantasy user was not found.";
        public static string FantasyUserGamesNotFound = "The fantasy user games were not found.";
        public static string TeamNotFound = "The team was not found.";
        public static string TeamStatsNotFound = "The team stats were not found.";
    }
}
