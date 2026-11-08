namespace Infrastructure.Persistence.Exceptions.Common
{
    public enum DbErrorCode
    {
        None = 0,
        UniqueViolation = 1,       
        ForeignKeyViolation = 2,   
        NotNullViolation = 3,      
        CheckViolation = 4,        
        Deadlock = 5,              
        Timeout = 6,              
        ConcurrencyViolation = 7,  
        Unknown = 99
    }
}
