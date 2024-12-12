using System.Net;
using System.Text.Json;
using System.Xml.Serialization;
using FbRider.Api.DTOs;
using FbRider.Api.YahooApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FbRider.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{

    // Error message constants
    public const string YahooApiErrorTitle = "Yahoo API error";
    public const string YahooApiErrorMessage = "An error occurred while processing your request.";
    public const string OAuthErrorTitle = "OAuth error";
    public const string GenericErrorTitle = "An error occurred";
    public const string GenericErrorMessage = "An internal server error occurred. Please try again later.";
    public const string LogUnhandledException = "An unhandled exception occurred.";

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (YahooApiException ex)
        {
            logger.LogError(ex, ex.ToString());
            await HandleYahooApiExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, LogUnhandledException);
            await HandleGenericExceptionAsync(context);
        }
    }

    private async Task HandleYahooApiExceptionAsync(HttpContext context, YahooApiException ex)
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
        else if(ex is {StatusCode: HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized})
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
        await context.Response.WriteAsync(JsonSerializer.Serialize(apiErrorResponse));
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

    private Task HandleGenericExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var apiErrorResponse = new ApiErrorResponse()
        {
            Error = GenericErrorTitle,
            Message = GenericErrorMessage
        };


        return context.Response.WriteAsync(JsonSerializer.Serialize(apiErrorResponse));
    }
}