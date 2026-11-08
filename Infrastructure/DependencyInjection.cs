using Application;
using Application.Cache.Common;
using Application.Exceptions;
using Application.Interfaces.Database;
using Domain.Abstractions.Repository;
using Domain.Abstractions.Repository.Common;
using Domain.Entities.Idempotency;
using Infrastructure.Cache;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Exceptions;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplication(configuration);

            var connectionString = PostgresConfig.BuildConnectionString(configuration) ?? throw new MissingFieldException("Please config the damn postgres connection string!");
            services.AddScoped<AuditInterceptor>();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
                options.UseNpgsql(connectionString)
                    .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));
            // Register check health
            services.AddHealthChecks()
                .AddNpgSql(connectionString, name: "myPostgres");
            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped<IIdempotentRepository<IdempotencyRecord, int>, IdempotentRepository>();
            services.AddSingleton<IDbExceptionTranslator, DbExceptionTranslate>();
            // Cache
            var redisConnectionString = configuration["Redis:ConnectionString"]
                            ?? throw new MissingFieldException("Please config the damn redis connection string!");
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddTransient(sp =>
                sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            // Register Repostiories
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
