using Domain.Abstractions.Repository;
using Domain.Entities.Idempotency;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class IdempotentRepository(ApplicationDbContext context)
                                : BaseRepository<IdempotencyRecord, int>(context), IIdempotentRepository<IdempotencyRecord, int>
    {
        public async Task<IdempotencyRecord?> GetByIdempotencyKey(
            string idempotencyKey,
            Guid? userId,
            string endpoint,
            CancellationToken cancellationToken)
        {
            return await dbSet.FirstOrDefaultAsync(
                x => x.Key == idempotencyKey && x.UserId == userId && x.Endpoint == endpoint,
                cancellationToken);
        }
    }
}
