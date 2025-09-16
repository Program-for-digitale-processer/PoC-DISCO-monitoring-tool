using System.Security.Claims;
using PoC_DISCO_Backend.Models;

namespace PoC_DISCO_Backend.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal ValidateToken(string token);
}