using Infrastructure.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Presentation.Middlewares
{
    public class GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                logger.LogWithLocation(
                    LogLevel.Information,
                    $"Request aborted by client: {context.Request.Path}");
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    logger.LogWithLocation(
                        LogLevel.Warning,
                        "Response already started, cannot write custom error body.",
                        ex);
                    throw;
                }

                var (statusCode, message) = ex switch
                {
                    KeyNotFoundException notFound => (
                        StatusCodes.Status404NotFound,
                        notFound.Message),

                    ArgumentException or InvalidOperationException => (
                        StatusCodes.Status400BadRequest,
                        ex.Message),

                    UnauthorizedAccessException unauthorized => (
                        StatusCodes.Status403Forbidden,
                        unauthorized.Message),

                    _ => (
                        StatusCodes.Status500InternalServerError,
                        "An unexpected error occurred. Please try again later.")
                };

                if (statusCode >= StatusCodes.Status500InternalServerError)
                {
                    logger.LogWithLocation(
                        LogLevel.Error,
                        $"Unhandled server error on {context.Request.Method} {context.Request.Path}: {ex.Message}",
                        ex);
                }
                else
                {
                    logger.LogWithLocation(
                        LogLevel.Information,
                        $"Client request error ({statusCode}) on {context.Request.Method} {context.Request.Path}: {message}");
                }

                var problem = new ProblemDetails
                {
                    Type = $"https://httpstatuses.io/{statusCode}",
                    Title = ReasonPhrases.GetReasonPhrase(statusCode),
                    Status = statusCode,
                    Detail = message,
                    Instance = context.Request.Path
                };
                problem.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
