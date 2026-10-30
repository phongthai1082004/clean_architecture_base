using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    public static class PostgresConfig
    {
        public static string GetDatabaseName(IConfiguration configuration) =>
            configuration.GetSection("PostgresConfig")["Database"] ?? "your_db_name";

        public static string BuildConnectionString(IConfiguration configuration, string? database = null)
        {
            var section = configuration.GetSection("PostgresConfig");
            return $"Host={section["Host"] ?? "localhost"};" +
                   $"Port={section["Port"] ?? "5432"};" +
                   $"Database={database ?? GetDatabaseName(configuration)};" +
                   $"Username={section["Username"] ?? "your_db_user"};" +
                   $"Password={section["Password"] ?? "your_db_password"}";
        }
    }
}
