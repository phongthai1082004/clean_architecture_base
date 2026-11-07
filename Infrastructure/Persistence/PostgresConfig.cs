using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    public static class PostgresConfig
    {
        public static string GetDatabaseName(IConfiguration configuration) =>
            Value(configuration.GetSection("PostgresConfig"), "Database", "your_db_name");

        public static string BuildConnectionString(IConfiguration configuration, string? database = null)
        {
            var section = configuration.GetSection("PostgresConfig");
            return $"Host={Value(section, "Host", "localhost")};" +
                   $"Port={Value(section, "Port", "5432")};" +
                   $"Database={(string.IsNullOrWhiteSpace(database) ? GetDatabaseName(configuration) : database)};" +
                   $"Username={Value(section, "Username", "your_db_user")};" +
                   $"Password={Value(section, "Password", "your_db_password")}";
        }

        private static string Value(IConfigurationSection section, string key, string fallback) =>
            string.IsNullOrWhiteSpace(section[key]) ? fallback : section[key]!;
    }
}
