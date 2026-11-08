namespace Application.Features.Authentication.Requests
{
    public record RegisterRequest(
        string Email,
        string FullName,
        string Password,
        string? PhoneNumber,
        string AvatarUrl
    );
}
