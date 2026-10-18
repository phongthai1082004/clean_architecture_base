using System.Text.Json;
using Application.Shared;

namespace Presentation.Middlewares
{
    public class GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                logger.LogInformation("Request aborted by client: {Path}", context.Request.Path);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);

                if (context.Response.HasStarted)
                {
                    logger.LogWarning("Response already started, cannot write error body.");
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Failure(
                    "An unexpected error occurred.",
                    StatusCodes.Status500InternalServerError);

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
            }
        }
    }
}
