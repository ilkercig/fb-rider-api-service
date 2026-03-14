using System.Net;

namespace FbRider.YahooApi;

public class YahooApiValidationException(
    string message,
    string endpoint,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    public string Endpoint { get; } = endpoint;

    public override string ToString()
    {
        return $"Message: {Message}, Endpoint: {Endpoint}, InnerException: {InnerException?.Message}";
    }
}
