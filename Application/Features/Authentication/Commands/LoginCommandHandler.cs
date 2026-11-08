using Application.CQRS;
using Application.Features.Authentication.Responses;
using Application.Interfaces.Services;
using Domain.Abstractions.Repository;
using Domain.Exceptions.Common;
using Domain.Enums.Users;
using Domain.Configurations;

namespace Application.Features.Authentication.Commands
{
    public class LoginCommandHandler(
        IJwtService jwtService,
        IUserRepository userRepository,
        JwtSettings jwtSettings) : ICommandHandler<LoginCommand, AuthResponse>
    {
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken = default)
        {
            var existingUser = await userRepository.GetUserByEmailAsync(request.Email, cancellationToken) ?? throw new NotFoundException($"User: {request.Email} is not found!"); ;
            if (existingUser.PasswordHash != jwtService.HashPassword(request.Password))
                throw new UnauthorizedAccessException($"User {request.Email} is not authorized!");
            if (existingUser.Status != UserStatus.Active)
                throw new BadRequestException($"User {request.Email} has been deactivated!");
            var accessToken = jwtService.GenerateToken(existingUser.Id.ToString(), existingUser.Email, existingUser.Role.ToString());
            var refreshToken = jwtService.GenerateRefreshToken();
            existingUser.AddToken(refreshToken, TokenType.RefreshToken, DateTimeOffset.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays));
            return new AuthResponse(accessToken, refreshToken);
        }
    }
}
