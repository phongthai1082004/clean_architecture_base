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

        public async Task<bool> CheckExistByRequestHash(string requestHash, CancellationToken cancellationToken)
        {
            return await dbSet.AnyAsync(x => x.RequestHash == requestHash, cancellationToken);
        }

        public async Task<IdempotencyRecord?> GetByIdempotencyKey(string idempotencyKey, CancellationToken cancellationToken)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.Key == idempotencyKey, cancellationToken);
        }
    }
}
