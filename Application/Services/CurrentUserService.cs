using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CurrentUserService> logger) : ICurrentUserService
    {
        private readonly HttpContext context = httpContextAccessor.HttpContext;
        private string? _cachedPayload;
        private bool _isPayloadRead;

        public Guid? UserId => Guid.TryParse(context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : null;
        public string? Idempotency => context?.Request.Headers["Idempotency-Key"].FirstOrDefault();
        public string Endpoint => context is not null
            ? $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}"
            : string.Empty;
        public string? Payload
        {
            get
            {
                if (_isPayloadRead) return _cachedPayload;

                var request = context?.Request;
                if (request?.Body == null)
                {
                    logger.LogDebug("Request payload unavailable");
                    _isPayloadRead = true;
                    return null;
                }

                if (!request.Body.CanSeek)
                {
                    var memoryStream = new MemoryStream();
                    request.Body.CopyTo(memoryStream);
                    request.Body = memoryStream;
                }

                request.Body.Position = 0;

                using var reader = new StreamReader(
                    request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true
                );

                _cachedPayload = reader.ReadToEnd();
                request.Body.Position = 0;
                _isPayloadRead = true;

                logger.LogDebug("Read request payload ({Length} chars)", _cachedPayload.Length);

                return _cachedPayload;
            }
        }
    }
}