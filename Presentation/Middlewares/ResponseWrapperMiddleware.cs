using System.Text.Json;
using Application.Shared;

namespace Presentation.Middlewares;

public class ResponseWrapperMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        if (path.StartsWith("/swagger") || path.StartsWith("/graphql"))
        {
            await next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        context.Response.Body = originalBodyStream;

        if (context.Response.StatusCode is >= 200 and < 300 && !string.IsNullOrWhiteSpace(responseBody))
        {
            object? parsedData;
            try
            {
                parsedData = JsonSerializer.Deserialize<object>(responseBody, JsonOptions);
            }
            catch
            {
                parsedData = responseBody;
            }

            var wrappedResponse = ApiResponse<object>.Success(
                data: parsedData ?? new object { },
                message: "Success.",
                statusCode: context.Response.StatusCode
            );

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(wrappedResponse, JsonOptions));
        }
        else
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
        }
    }
}