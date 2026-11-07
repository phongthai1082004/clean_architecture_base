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
using System.Text.Json;

namespace Application.Behaviors
{
    public class IdempotencyBehavior<TRequest, TResponse>(
        ICacheService cache,
        ICurrentUserService currentUser,
        IIdempotentRepository<IdempotencyRecord, Guid> idempotentRepository)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IIdempotentCommand<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var idempotencyKeyRaw = currentUser.Idempotency ?? throw new MissingIdempotencyKeyException();
            var requestHash = IdempotencyRecord.ComputeRequestHash(
                idempotencyKeyRaw,
                currentUser.Endpoint,
                currentUser.UserId,
                currentUser.Payload);

            var shortDataKey = CacheKeys.IdempotencyShortData<TRequest>(idempotencyKeyRaw);
            var lockKey = CacheKeys.IdempotencyLockData<string>(idempotencyKeyRaw);

            var cacheExistingIdempotency = await cache.GetAsync<IdempotencyRecord>(shortDataKey);
            if (cacheExistingIdempotency != null)
            {
                if (!string.Equals(cacheExistingIdempotency.RequestHash, requestHash, StringComparison.Ordinal))
                {
                    throw new UnprocessableEntityException("There's something horrible happen to your payload. Please try again!");
                }

                if (cacheExistingIdempotency.Status == IdempotencyStatus.Completed)
                {
                    return DeserializeResponse(cacheExistingIdempotency.ResponseBody);
                }

                throw new ConflictException("A request with the same idempotency key is currently being processed.");
            }

            var existingIdempotency = await idempotentRepository.GetByIdempotencyKey(idempotencyKeyRaw, cancellationToken);
            if (existingIdempotency != null)
            {
                if (!string.Equals(existingIdempotency.RequestHash, requestHash, StringComparison.Ordinal))
                {
                    throw new UnprocessableEntityException("There's something horrible happen to your payload. Please try again!");
                }

                if (existingIdempotency.Status == IdempotencyStatus.Pending)
                {
                    if (existingIdempotency.IsExpired())
                    {
                        existingIdempotency.MarkFailed();
                        idempotentRepository.Update(existingIdempotency);
                        await idempotentRepository.SaveChangesAsync(cancellationToken);
                        throw new TimeoutException("The operation takes too long to process and it's failed!");
                    }

                    throw new ConflictException("A request with the same idempotency key is currently being processed.");
                }
                else if (existingIdempotency.Status == IdempotencyStatus.Failed)
                {
                    throw new InvalidOperationException("The previous operation with this idempotency key failed and cannot be retried.");
                }
                else if (existingIdempotency.Status == IdempotencyStatus.Completed)
                {
                    await cache.SetAsync(shortDataKey, existingIdempotency, TimeSpan.FromDays(1));
                    return DeserializeResponse(existingIdempotency.ResponseBody);
                }
            }

            bool isLocked = await cache.SetAsync(
                lockKey,
                idempotencyKeyRaw,
                TimeSpan.FromSeconds(30),
                CacheCondition.NotExists);

            if (!isLocked)
            {
                throw new ConflictException("A request with the same idempotency key is currently being processed.");
            }

            var record = new IdempotencyRecord(
                idempotencyKeyRaw,
                currentUser.UserId,
                currentUser.Endpoint,
                requestHash,
                DateTime.UtcNow.AddDays(1)
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

                record.Complete(200, responseBody, "application/json");
                idempotentRepository.Update(record);
                await idempotentRepository.SaveChangesAsync(cancellationToken);

                await cache.SetAsync(shortDataKey, record, TimeSpan.FromDays(1));

                return response;
            }
            catch (Exception)
            {
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
                    args: [idempotencyKeyRaw]
                );
            }
        }

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