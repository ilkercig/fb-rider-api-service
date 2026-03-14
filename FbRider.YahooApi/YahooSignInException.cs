using System.Net;

namespace FbRider.YahooApi;

public class YahooSignInException(
    string message,
    string endpoint,
    string responseContent,
    HttpStatusCode statusCode,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    public string Endpoint { get; } = endpoint;
    public string ResponseContent { get; } = responseContent;
    public HttpStatusCode StatusCode { get; } = statusCode;

    public override string ToString()
    {
        return
            $"Message: {Message}, Endpoint: {Endpoint}, StatusCode: {StatusCode}, ResponseContent: {ResponseContent}, InnerException: {InnerException?.Message}";
    }
}
