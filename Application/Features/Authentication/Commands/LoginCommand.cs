using Application.CQRS;
using Application.Features.Authentication.Responses;

namespace Application.Features.Authentication.Commands
{
    public record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;
}
