using MediatR;
using Microsoft.AspNetCore.Identity;
using SignaliteWebAPI.Domain.DTOs.Auth;
using SignaliteWebAPI.Infrastructure.Exceptions;
using SignaliteWebAPI.Infrastructure.Interfaces.Repositories;
using SignaliteWebAPI.Infrastructure.Interfaces.Services;

namespace SignaliteWebAPI.Application.Features.Auth.Login;

public class LoginUserHandler(
    IUserRepository userRepository,
    IPasswordHasher<Domain.Models.User> passwordHasher,
    ITokenService tokenService
    ): IRequestHandler<LoginCommand, LoginResponseDTO>
{
    public async Task<LoginResponseDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmail(request.LoginDto.Email);
        
        if (user == null)
            throw new AuthException("Invalid email or password");
            
        // Verify password
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(
            user, user.HashedPassword, request.LoginDto.Password);
            
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            throw new AuthException("Invalid email or password");
            
        // Generate tokens
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        
        // Update the refresh token in database
        await userRepository.UpdateRefreshToken(user.Id, refreshToken, refreshTokenExpiry);
        
        return new LoginResponseDTO
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddDays(1) // This should match your access token expiry
        };
    }
}