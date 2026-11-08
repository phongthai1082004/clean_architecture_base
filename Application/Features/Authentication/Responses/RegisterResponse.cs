using Domain.Enums.Users;

namespace Application.Features.Authentication.Responses
{
    public record RegisterResponse(
        string Email,
        string FullName,
        string? PhoneNumber,
        string AvatarUrl,
        UserStatus Status,
        UserRole Role
    );  
}
