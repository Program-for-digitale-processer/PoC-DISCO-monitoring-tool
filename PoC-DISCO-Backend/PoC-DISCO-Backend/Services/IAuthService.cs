using PoC_DISCO_Backend.Dtos;

namespace PoC_DISCO_Backend.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
}