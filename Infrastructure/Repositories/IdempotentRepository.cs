using Domain.Abstractions.Repository;
using Domain.Entities.Idempotency;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class IdempotentRepository(ApplicationDbContext context, ILoggerFactory loggerFactory)
                                : BaseRepository<IdempotencyRecord, int>(context, loggerFactory), IIdempotentRepository<IdempotencyRecord, int>
    {
        public async Task<IdempotencyRecord?> GetByIdempotencyKey(
            string idempotencyKey,
            Guid? userId,
            string endpoint,
            CancellationToken cancellationToken)
        {
            var record = await dbSet.FirstOrDefaultAsync(
                x => x.Key == idempotencyKey && x.UserId == userId && x.Endpoint == endpoint,
                cancellationToken);

            logger.LogDebug(
                "Idempotency lookup key={IdempotencyKey} user={UserId} endpoint={Endpoint} found={Found}",
                idempotencyKey, userId, endpoint, record is not null);

            return record;
        }
    }
}
