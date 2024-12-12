using System.Net;

namespace FbRider.Api.YahooApi;

public class YahooApiException(
    string message,
    string endpoint,
    YahooApiType apiType,
    string? responseContent = null,
    HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    public string Endpoint { get; } = endpoint;
    public string? ResponseContent { get; } = responseContent;
    public HttpStatusCode StatusCode { get; } = statusCode;

    public YahooApiType ApiType { get; } = apiType;

    public override string ToString()
    {
        return
            $"Message: {Message}, Endpoint: {Endpoint}, StatusCode: {StatusCode}, ResponseContent: {ResponseContent}, InnerException: {InnerException?.Message}";
    }
}