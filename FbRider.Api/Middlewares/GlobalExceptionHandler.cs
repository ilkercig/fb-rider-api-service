using System.Net;
using System.Text.Json;
using System.Xml.Serialization;
using FbRider.Api.DTOs;
using FbRider.YahooApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;

namespace FbRider.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // Error message constants
    public const string YahooApiErrorTitle = "Yahoo API error";
    public const string YahooApiErrorMessage = "An error occurred while processing your request.";
    public const string OAuthErrorTitle = "OAuth error";
    public const string GenericErrorTitle = "An error occurred";
    public const string GenericErrorMessage = "An internal server error occurred. Please try again later.";
    public const string LogUnhandledException = "An unhandled exception occurred.";

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is YahooApiException yahooApiException)
        {
            logger.LogError(yahooApiException, yahooApiException.ToString());
            await HandleYahooApiExceptionAsync(httpContext, yahooApiException, cancellationToken);
            return true;
        }

        logger.LogError(exception, LogUnhandledException);
        await HandleGenericExceptionAsync(httpContext, cancellationToken);
        return true;
    }

    private async Task HandleYahooApiExceptionAsync(HttpContext context, YahooApiException ex, CancellationToken cancellationToken)
    {
        Action<string, ApiErrorResponse> extractErrorMessage = ex.ApiType == YahooApiType.SignIn ? ExtractOAuthError
            : ExtractYahooApiError;
        var apiErrorResponse = new ApiErrorResponse()
        {
            Error = YahooApiErrorTitle,
            Message = YahooApiErrorMessage
        };

        if (ex is { StatusCode: HttpStatusCode.BadRequest, ResponseContent: not null })
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            extractErrorMessage(ex.ResponseContent, apiErrorResponse);
        }
        else if (ex is { StatusCode: HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized })
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            apiErrorResponse.Error = "User is not signed in";
            if (context.User.Identity is { IsAuthenticated: true })
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(apiErrorResponse), cancellationToken);
    }

    private void ExtractOAuthError(string responseContent, ApiErrorResponse errorResponse)
    {
        try
        {
            var oAuthError = JsonSerializer.Deserialize<OAuthErrorResponse>(responseContent);
            if (oAuthError != null)
            {
                errorResponse.Error = oAuthError.Error ?? OAuthErrorTitle;
                errorResponse.Message = oAuthError.ErrorDescription ?? YahooApiErrorMessage;
            }
        }
        catch (Exception)
        {
            logger.LogError("Unknown error response.");
        }
    }

    private void ExtractYahooApiError(string responseContent, ApiErrorResponse errorResponse)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(YahooApiError));
            using var reader = new StringReader(responseContent);
            var yahooApiError = (YahooApiError?)serializer.Deserialize(reader);

            if (yahooApiError != null)
            {
                errorResponse.Error = yahooApiError.Description ?? YahooApiErrorTitle;
                errorResponse.Message = yahooApiError.Detail ?? YahooApiErrorMessage;
            }
        }
        catch (Exception)
        {
            logger.LogError("Unknown error response.");
        }
    }

    private Task HandleGenericExceptionAsync(HttpContext context, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var apiErrorResponse = new ApiErrorResponse()
        {
            Error = GenericErrorTitle,
            Message = GenericErrorMessage
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(apiErrorResponse), cancellationToken);
    }
}
