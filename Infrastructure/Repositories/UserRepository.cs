using Domain.Abstractions.Repository;
using Domain.Entities.Users;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class UserRepository(
        ApplicationDbContext context,
        ILoggerFactory loggerFactory) : BaseRepository<User, Guid>(context, loggerFactory), IUserRepository
    {
        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await dbSet.FirstOrDefaultAsync(user => user.Email == email,cancellationToken);
        }
        public async Task<bool> CheckUserByEmail(string email, CancellationToken cancellationToken = default)
        {
            return await dbSet.AnyAsync(user => user.Email == email, cancellationToken);
        }
    }
}
