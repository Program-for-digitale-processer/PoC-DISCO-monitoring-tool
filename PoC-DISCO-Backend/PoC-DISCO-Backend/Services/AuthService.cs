using PoC_DISCO_Backend.Dtos;
using Microsoft.EntityFrameworkCore;
using PoC_DISCO_Backend.Data;
using PoC_DISCO_Backend.Models;

namespace PoC_DISCO_Backend.Services;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly DiscoToolContext _context;

    public AuthService(DiscoToolContext context, IJwtService jwtService)
    {
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
        if (user is null)
            return null;

        var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if(!isPasswordCorrect)
            return null;
        
        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        
        return new AuthResponseDto()
        {
            Token = _jwtService.GenerateToken(user),
            User = new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                RefreshToken = user.RefreshToken
            }
        };
    }

    public async Task<AuthResponseDto?> GetRefreshTokenAsync(TokenDto tokenDto)
    {
        var principal = _jwtService.ValidateToken(tokenDto.AccessToken);
        var username = principal.Identity?.Name;
        var user = _context.Users.SingleOrDefault(u => u.UserName == username);

        if (user is null || user.RefreshToken != tokenDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return null;

        var newAccessToken = _jwtService.GenerateToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new AuthResponseDto()
        {
            Token = newAccessToken,
            User = new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                RefreshToken = user.RefreshToken
            }
        };
    }
    
    public async Task<User?> RevokeRefreshTokenAsync(string? userName)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserName == userName);
        if (user == null) return null;
            
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        
        await _context.SaveChangesAsync();
        
        return user;
    }
}