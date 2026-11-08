namespace Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, string role);
        string GenerateRefreshToken();
        string HashPassword(string password);
        bool VerifyPassword(string providedPassword, string hashedPassword);
    }
}
