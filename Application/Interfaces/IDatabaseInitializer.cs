namespace Application.Interfaces
{
    public interface IDatabaseInitializer
    {
        Task MigrateAsync();
    }
}