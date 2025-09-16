using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PoC_DISCO_Backend.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly IConfiguration _jwtConfig;
    private readonly byte[] _secretKey;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        _jwtConfig = _configuration.GetSection("Jwt");
        _secretKey = Encoding.UTF8.GetBytes(_jwtConfig["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing"));
        _tokenHandler = new JwtSecurityTokenHandler();

    }

    public string GenerateToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            ]),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKey),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtConfig["Issuer"],
            Audience = _jwtConfig["Audience"]
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_secretKey),
            ValidateIssuer = true,
            ValidIssuer = _jwtConfig["Issuer"],
            ValidateAudience = true,
            ValidAudience = _jwtConfig["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
        
        var principal = _tokenHandler.ValidateToken(token, validationParameters, out _);
        return principal;
    }
}