using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.DTOs;
using TradingPlatform.Application.Exceptions;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Application.Services;

public class AuthService(
    ITradingDbContext db,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Username, email, and password are required.");
        }

        if (request.Password.Length < 6)
        {
            throw new ValidationException("Password must be at least 6 characters.");
        }

        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email))
        {
            throw new ValidationException("Email is already registered.");
        }

        if (await db.Users.AnyAsync(u => u.Username == request.Username.Trim()))
        {
            throw new ValidationException("Username is already taken.");
        }

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return new AuthResponse(jwtTokenService.GenerateToken(user), user.Username, user.Email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        return new AuthResponse(jwtTokenService.GenerateToken(user), user.Username, user.Email);
    }
}
