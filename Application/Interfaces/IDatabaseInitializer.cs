namespace Application.Interfaces
{
    /// <summary>Applies pending EF Core migrations at application startup.</summary>
    public interface IDatabaseInitializer
    {
        /// <summary>Applies any pending migrations, if the database is not up to date.</summary>
        Task MigrateAsync();
    }
}