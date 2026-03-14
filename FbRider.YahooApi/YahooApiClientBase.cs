using System.Net;
using Polly;
using Polly.Retry;

namespace FbRider.YahooApi
{
    public abstract class YahooApiClientBase<TException>(HttpClient httpClient, ResiliencePipeline<HttpResponseMessage> resiliencePipeline)
        where TException : Exception
    {
        protected abstract TException CreateException(string message, string endpoint, string responseContent, HttpStatusCode statusCode, Exception? innerException = null);

        protected async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            string responseContent = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var requestUri = request.RequestUri?.ToString() ?? "";
            HttpResponseMessage? response = null;
            try
            {
                response = await resiliencePipeline.ExecuteAsync(async token =>
                {
                    var attemptRequest = await CloneHttpRequestMessageAsync(request);
                    return await httpClient.SendAsync(attemptRequest, token);
                });
                statusCode = response.StatusCode;
                responseContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw CreateException(YahooApiErrorMessages.ApiRequestError, requestUri, responseContent, statusCode, innerException: ex);
            }

            if (!response.IsSuccessStatusCode)
                throw CreateException(YahooApiErrorMessages.ResponseNotSuccessful, requestUri, responseContent,
                    response.StatusCode);

            T? result;
            try
            {
                result = Deserialize<T>(responseContent);
            }
            catch (Exception ex)
            {
                throw CreateException(YahooApiErrorMessages.DeserializationFailed, requestUri, responseContent,
                    response.StatusCode, ex);
            }

            if (result == null)
                throw CreateException(YahooApiErrorMessages.DeserializationFailed, requestUri, responseContent,
                    response.StatusCode);

            return result;
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version
            };

            foreach (var prop in request.Options)
            {
                clone.Options.Set(new HttpRequestOptionsKey<object?>(prop.Key), prop.Value);
            }

            if (request.Content != null)
            {
                var contentBytes = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(contentBytes);

                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }

        protected abstract T? Deserialize<T>(string responseContent);
    }
}
