using Application.Interfaces;
using FluentMigrator.Runner;
using Infrastructure.Abstractions;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data.Common;
using System.Reflection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = PostgresConfig.BuildConnectionString(configuration);

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            services.AddScoped<DbConnection>(_ => new NpgsqlConnection(connectionString));
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
