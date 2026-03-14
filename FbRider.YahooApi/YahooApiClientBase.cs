using System.Net;
using Polly;
using Polly.Retry;

namespace FbRider.YahooApi
{
    public abstract class YahooApiClientBase(HttpClient httpClient)
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

        protected async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            string responseContent;
            var requestUri = request.RequestUri?.ToString() ?? "";
            HttpResponseMessage response;
            try
            {
                response = await ResiliencePipeline.ExecuteAsync(async token =>
                {
                    var attemptRequest = await CloneHttpRequestMessageAsync(request);
                    return await httpClient.SendAsync(attemptRequest, token);
                });
                responseContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new YahooApiException(YahooApiErrorMessages.ApiRequestError, requestUri, ApiType, innerException: ex);
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

        protected abstract YahooApiType ApiType { get; }

        protected abstract T? Deserialize<T>(string responseContent);
    }
}
