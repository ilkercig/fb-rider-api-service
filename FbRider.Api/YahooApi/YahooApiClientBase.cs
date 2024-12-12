namespace FbRider.Api.YahooApi
{
    public abstract class YahooApiClientBase(HttpClient httpClient)
    {
        protected async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            string responseContent;
            var requestUri = request.RequestUri?.ToString() ?? "";
            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(request);
                responseContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new YahooApiException(YahooApiErrorMessages.ApiRequestError, requestUri, ApiType,  innerException: ex);
            }

            if (!response.IsSuccessStatusCode)
                throw new YahooApiException(YahooApiErrorMessages.ResponseNotSuccessful, requestUri, ApiType, responseContent,
                    response.StatusCode);

            T? result;
            try
            {
                result = Deserialize<T>(responseContent);
            }
            catch (Exception ex)
            {
                throw new YahooApiException(YahooApiErrorMessages.DeserializationFailed, requestUri, ApiType, responseContent,
                    response.StatusCode, ex);
            }

            if (result == null)
                throw new YahooApiException(YahooApiErrorMessages.DeserializationFailed, requestUri, ApiType, responseContent,
                    response.StatusCode);

            return result;
        }

        protected abstract YahooApiType ApiType { get; }

        protected abstract T? Deserialize<T>(string responseContent);
    }
}
