using PoC_DISCO_Backend.Dtos;
using Microsoft.EntityFrameworkCore;
using PoC_DISCO_Backend.Data;

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
        
        return new AuthResponseDto()
        {
            Token = _jwtService.GenerateToken(user),
            User = new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
            }
        };
    }
}