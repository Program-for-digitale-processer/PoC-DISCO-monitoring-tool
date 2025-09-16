using PoC_DISCO_Backend.Dtos;
using PoC_DISCO_Backend.Models;

namespace PoC_DISCO_Backend.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> GetRefreshTokenAsync(TokenDto tokenDto);
    Task<User?> RevokeRefreshTokenAsync(string? userName);
}