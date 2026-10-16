using Application.Interfaces;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Configurations
{
    public class DatabaseInitializer(IServiceScopeFactory scopeFactory) : IDatabaseInitializer
    {
        public async Task MigrateAsync()
        {
            using var scope = scopeFactory.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
            await Task.CompletedTask;
        }
    }
}