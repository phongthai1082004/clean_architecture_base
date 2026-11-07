using Application.Cache.Common;
using Application.Interfaces;
using Infrastructure.Cache;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using StackExchange.Redis;
using System.Data.Common;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = PostgresConfig.BuildConnectionString(configuration) ?? throw new MissingFieldException("Please config the damn postgres connection string!");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbConnection>(_ => new NpgsqlConnection(connectionString));
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            services.AddScoped<ICacheService, CacheService>();

            // Cache
            var redisConnectionString = configuration["Redis:ConnectionString"]
                            ?? throw new MissingFieldException("Please config the damn redis connection string!");
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddTransient(sp =>
                sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            return services;
        }
    }
}
