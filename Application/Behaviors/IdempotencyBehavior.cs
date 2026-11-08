using Application.Cache.Common;
using Application.CQRS;
using Application.Interfaces;
using Domain.Abstractions.Repository;
using Domain.Entities.Idempotency;
using Domain.Enums;
using Domain.Enums.Idempotency;
using Domain.Exceptions;
using Domain.Exceptions.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Behaviors
{
    public class IdempotencyBehavior<TRequest, TResponse>(
        ICacheService cache,
        ICurrentUserService currentUser,
        IIdempotentRepository<IdempotencyRecord, int> idempotentRepository,
        ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IIdempotentCommand<TResponse>
    {
        private static readonly TimeSpan PendingTtl = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan CompletedTtl = TimeSpan.FromDays(1);

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var idempotencyKey = currentUser.Idempotency ?? throw new MissingIdempotencyKeyException();
            var requestHash = IdempotencyRecord.ComputeRequestHash(
                currentUser.Endpoint,
                currentUser.UserId,
                currentUser.Payload);
            if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Idempotency check {RequestName} key={IdempotencyKey} user={UserId} endpoint={Endpoint}", typeof(TRequest).Name, idempotencyKey, currentUser.UserId, currentUser.Endpoint);
            var dataKey = CacheKeys.IdempotencyShortData<TRequest>(idempotencyKey, currentUser.UserId, currentUser.Endpoint);
            var lockKey = CacheKeys.IdempotencyLockData<TRequest>(idempotencyKey, currentUser.UserId, currentUser.Endpoint);

            var cached = await cache.GetAsync<IdempotencyCacheEntry>(dataKey);
            if (cached is not null)
            {
                if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Idempotency cache hit for {RequestName}", typeof(TRequest).Name);

                if (!string.Equals(cached.RequestHash, requestHash, StringComparison.Ordinal))
                {
                    logger.LogWarning("Idempotency payload mismatch (cache) for {RequestName}", typeof(TRequest).Name);
                    throw new UnprocessableEntityException("There's something horrible happen to your payload. Please try again!");
                }

                if (cached.Status == IdempotencyStatus.Completed)
                {
                    logger.LogInformation("Replaying idempotent response for {RequestName}", typeof(TRequest).Name);
                    return DeserializeResponse(cached.ResponseBody);
                }

                throw new ConflictException("A request with the same idempotency key is currently being processed.");
            }

            var existing = await idempotentRepository.GetByIdempotencyKey(
                idempotencyKey,
                currentUser.UserId,
                currentUser.Endpoint,
                cancellationToken);
            if (existing is not null)
            {
                if (logger.IsEnabled(LogLevel.Debug))  logger.LogDebug("Idempotency DB hit for {RequestName} status={Status}", typeof(TRequest).Name, existing.Status);

                if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
                {
                    logger.LogWarning("Idempotency payload mismatch (DB) for {RequestName}", typeof(TRequest).Name);
                    throw new UnprocessableEntityException("There's something horrible happen to your payload. Please try again!");
                }

                if (existing.Status == IdempotencyStatus.Completed)
                {
                    if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Replaying idempotent response for {RequestName}", typeof(TRequest).Name);
                    await cache.SetAsync(dataKey, ToEntry(existing), CompletedTtl);
                    return DeserializeResponse(existing.ResponseBody);
                }

                if (existing.Status == IdempotencyStatus.Failed || existing.IsExpired())
                {
                    if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Resetting idempotency record for retry {RequestName} (status={Status})", typeof(TRequest).Name, existing.Status);
                    idempotentRepository.Delete(existing);
                    await idempotentRepository.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw new ConflictException("A request with the same idempotency key is currently being processed.");
                }
            }

            bool isLocked = await cache.SetAsync(
                lockKey,
                idempotencyKey,
                TimeSpan.FromSeconds(30),
                CacheCondition.NotExists);

            if (!isLocked)
            {
                if (logger.IsEnabled(LogLevel.Debug)) logger.LogDebug("Idempotency lock contention for {RequestName}", typeof(TRequest).Name);
                throw new ConflictException("A request with the same idempotency key is currently being processed.");
            }

            if (logger.IsEnabled(LogLevel.Debug))  logger.LogDebug("Idempotency lock acquired for {RequestName}", typeof(TRequest).Name);

            var record = new IdempotencyRecord(
                idempotencyKey,
                currentUser.UserId,
                currentUser.Endpoint,
                requestHash,
                DateTime.UtcNow.Add(PendingTtl)
            );

            idempotentRepository.Insert(record);
            await idempotentRepository.SaveChangesAsync(cancellationToken);

            try
            {
                var response = await next();

                string? responseBody = null;
                if (typeof(TResponse) != typeof(Unit))
                {
                    responseBody = JsonSerializer.Serialize(response);
                }

                var statusCode = typeof(TResponse) == typeof(Unit) ? 204 : 200;
                record.Complete(statusCode, responseBody, "application/json");
                idempotentRepository.Update(record);
                await idempotentRepository.SaveChangesAsync(cancellationToken);

                await cache.SetAsync(dataKey, ToEntry(record), CompletedTtl);

                if (logger.IsEnabled(LogLevel.Information))  logger.LogInformation("Completed idempotent request {RequestName} status={StatusCode}", typeof(TRequest).Name, statusCode);

                return response;
            }
            catch (Exception)
            {
                logger.LogWarning("Idempotent request {RequestName} failed; marking record Failed", typeof(TRequest).Name);
                record.MarkFailed();
                idempotentRepository.Update(record);
                await idempotentRepository.SaveChangesAsync(cancellationToken);
                throw;
            }
            finally
            {
                const string releaseLockScript = @"
                    if redis.call('get', KEYS[1]) == ARGV[1] then
                        return redis.call('del', KEYS[1])
                    else
                        return 0
                    end";

                await cache.ExecuteScriptAsync(
                    releaseLockScript,
                    keys: [lockKey],
                    args: [idempotencyKey]
                );
            }
        }

        private static IdempotencyCacheEntry ToEntry(IdempotencyRecord record) =>
            new(record.RequestHash ?? string.Empty, record.Status, record.ResponseBody);

        private static TResponse DeserializeResponse(string? responseBody)
        {
            if (typeof(TResponse) == typeof(Unit) || string.IsNullOrWhiteSpace(responseBody))
            {
                return (TResponse)(object)Unit.Value;
            }

            return JsonSerializer.Deserialize<TResponse>(responseBody)!;
        }
    }
}
