using Application.Interfaces;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Persistence.Configurations
{
    public class DatabaseInitializer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
    {
        public async Task MigrateAsync()
        {
            await EnsureDatabaseExistsAsync();

            using var scope = scopeFactory.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
            logger.LogInformation("Database migration completed successfully.");
        }

        private async Task EnsureDatabaseExistsAsync()
        {
            var database = PostgresConfig.GetDatabaseName(configuration);
            var maintenanceConnectionString = PostgresConfig.BuildConnectionString(configuration, "postgres");

            await using var connection = new NpgsqlConnection(maintenanceConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM pg_database WHERE datname = @databaseName";
            command.Parameters.AddWithValue("@databaseName", database);

            var exists = await command.ExecuteScalarAsync() != null;

            if (exists) return;

            logger.LogInformation("Database '{DatabaseName}' does not exist. Creating...", database);

            await using var createCommand = connection.CreateCommand();
            createCommand.CommandText = $"CREATE DATABASE \"{database}\"";
            await createCommand.ExecuteNonQueryAsync();

            logger.LogInformation("Database '{DatabaseName}' created successfully.", database);
        }
    }
}
