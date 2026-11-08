
using Application.CQRS;
using Application.Features.Authentication.Responses;
using Application.Interfaces.Services;
using Domain.Abstractions.Repository;
using Domain.Entities.Users;
using Domain.Exceptions.Common;

namespace Application.Features.Authentication.Commands
{
    public class RegisterCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService) : ICommandHandler<RegisterCommand, RegisterResponse>
    {
        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await userRepository.CheckUserByEmail(request.Email, cancellationToken)) throw new ConflictException($"User: {request.Email} is existed!");
            var user = new User(request.Email, request.FullName, jwtService.HashPassword(request.Password), request.PhoneNumber);
            userRepository.Insert(user);
            return new RegisterResponse(user.Email, user.FullName, user.PhoneNumber, user.AvatarUrl, user.Status, user.Role);
        }
    }
}
