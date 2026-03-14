using System.Net;
using System.Text.Json;
using System.Xml.Serialization;
using FbRider.Api.DTOs;
using FbRider.YahooApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FbRider.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public const string YahooApiErrorTitle = "Yahoo API error";
    public const string YahooApiErrorMessage = "An error occurred while processing your request.";
    public const string OAuthErrorTitle = "OAuth error";
    public const string GenericErrorTitle = "An error occurred";
    public const string GenericErrorMessage = "An internal server error occurred. Please try again later.";
    public const string LogUnhandledException = "An unhandled exception occurred.";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is YahooApiValidationException validationException)
        {
            logger.LogWarning(validationException, validationException.ToString());
            await HandleYahooApiValidationExceptionAsync(httpContext, validationException, cancellationToken);
            return true;
        }

        if (exception is YahooFantasySportsException fantasySportsException)
        {
            logger.LogError(fantasySportsException, fantasySportsException.ToString());
            await HandleYahooFantasySportsExceptionAsync(httpContext, fantasySportsException, cancellationToken);
            return true;
        }

        if (exception is YahooSignInException signInException)
        {
            logger.LogError(signInException, signInException.ToString());
            await HandleYahooSignInExceptionAsync(httpContext, signInException, cancellationToken);
            return true;
        }

        logger.LogError(exception, LogUnhandledException);
        await HandleGenericExceptionAsync(httpContext, cancellationToken);
        return true;
    }

    private async Task HandleYahooApiValidationExceptionAsync(HttpContext context, YahooApiValidationException ex, CancellationToken cancellationToken)
    {
        var problem = CreateProblemDetails(
            (int)HttpStatusCode.BadRequest,
            YahooApiErrorTitle,
            ex.Message);

        await WriteProblemDetailsAsync(context, problem, cancellationToken);
    }

    private async Task HandleYahooFantasySportsExceptionAsync(HttpContext context, YahooFantasySportsException ex, CancellationToken cancellationToken)
    {
        ProblemDetails problem;

        if (ex is { StatusCode: HttpStatusCode.BadRequest })
        {
            var (title, detail) = ExtractYahooApiError(ex.ResponseContent);
            problem = CreateProblemDetails((int)HttpStatusCode.BadRequest, title, detail);
        }
        else if (ex is { StatusCode: HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized })
        {
            problem = CreateProblemDetails((int)HttpStatusCode.Unauthorized, "User is not signed in", YahooApiErrorMessage);
            if (context.User.Identity is { IsAuthenticated: true })
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        else
        {
            problem = CreateProblemDetails((int)HttpStatusCode.InternalServerError, YahooApiErrorTitle, YahooApiErrorMessage);
        }

        await WriteProblemDetailsAsync(context, problem, cancellationToken);
    }

    private async Task HandleYahooSignInExceptionAsync(HttpContext context, YahooSignInException ex, CancellationToken cancellationToken)
    {
        ProblemDetails problem;

        if (ex is { StatusCode: HttpStatusCode.BadRequest })
        {
            var (title, detail) = ExtractOAuthError(ex.ResponseContent);
            problem = CreateProblemDetails((int)HttpStatusCode.BadRequest, title, detail);
        }
        else if (ex is { StatusCode: HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized })
        {
            problem = CreateProblemDetails((int)HttpStatusCode.Unauthorized, "User is not signed in", YahooApiErrorMessage);
            if (context.User.Identity is { IsAuthenticated: true })
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        else
        {
            problem = CreateProblemDetails((int)HttpStatusCode.InternalServerError, YahooApiErrorTitle, YahooApiErrorMessage);
        }

        await WriteProblemDetailsAsync(context, problem, cancellationToken);
    }

    private async Task HandleGenericExceptionAsync(HttpContext context, CancellationToken cancellationToken)
    {
        var problem = CreateProblemDetails(
            (int)HttpStatusCode.InternalServerError,
            GenericErrorTitle,
            GenericErrorMessage);

        await WriteProblemDetailsAsync(context, problem, cancellationToken);
    }

    private (string title, string detail) ExtractOAuthError(string responseContent)
    {
        try
        {
            var oAuthError = JsonSerializer.Deserialize<OAuthErrorResponse>(responseContent);
            if (oAuthError != null)
                return (oAuthError.Error ?? OAuthErrorTitle, oAuthError.ErrorDescription ?? YahooApiErrorMessage);
        }
        catch (Exception)
        {
            logger.LogError("Unknown error response.");
        }

        return (OAuthErrorTitle, YahooApiErrorMessage);
    }

    private (string title, string detail) ExtractYahooApiError(string responseContent)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(YahooApiError));
            using var reader = new StringReader(responseContent);
            var yahooApiError = (YahooApiError?)serializer.Deserialize(reader);
            if (yahooApiError != null)
                return (yahooApiError.Description ?? YahooApiErrorTitle, yahooApiError.Detail ?? YahooApiErrorMessage);
        }
        catch (Exception)
        {
            logger.LogError("Unknown error response.");
        }

        return (YahooApiErrorTitle, YahooApiErrorMessage);
    }

    private static ProblemDetails CreateProblemDetails(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };

    private static async Task WriteProblemDetailsAsync(HttpContext context, ProblemDetails problem, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = problem.Status!.Value;
        context.Response.ContentType = "application/problem+json";
        await JsonSerializer.SerializeAsync(context.Response.Body, problem, JsonOptions, cancellationToken);
    }
}
