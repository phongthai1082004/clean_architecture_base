using Application.CQRS;
using Application.Features.Authentication.Responses;

namespace Application.Features.Authentication.Commands
{
    public record RegisterCommand(
        string Email,
        string FullName,
        string Password,
        string? PhoneNumber,
        string AvatarUrl
    ) : ICommand<RegisterResponse>;
}
