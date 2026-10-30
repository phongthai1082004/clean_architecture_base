using Application.Interfaces;
using Domain.Abstractions.Repository;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data.Common;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = PostgresConfig.BuildConnectionString(configuration);

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<DbConnection>(_ => new NpgsqlConnection(connectionString));
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
