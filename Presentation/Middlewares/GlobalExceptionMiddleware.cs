using Domain.Exceptions.Common;
using Domain.Exceptions.Idempotency;
using Infrastructure.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

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

                    FluentValidation.ValidationException validation => (
                        StatusCodes.Status400BadRequest,
                        string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))),

                    MissingIdempotencyKeyException missingKey => (
                        StatusCodes.Status400BadRequest,
                        missingKey.Message),

                    UnauthorizedAccessException unauthorized => (
                        StatusCodes.Status403Forbidden,
                        unauthorized.Message),

                    ConflictException or IdempotencyProcessingException => (
                        StatusCodes.Status409Conflict,
                        ex.Message),

                    UnprocessableEntityException unprocessable => (
                        StatusCodes.Status422UnprocessableEntity,
                        unprocessable.Message),

                    OperationCanceledException or TimeoutException => (
                        StatusCodes.Status504GatewayTimeout,
                        "The request timed out."),

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
