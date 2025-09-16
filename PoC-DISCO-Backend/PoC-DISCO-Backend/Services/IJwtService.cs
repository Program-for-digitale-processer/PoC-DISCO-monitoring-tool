using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace PoC_DISCO_Backend.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}