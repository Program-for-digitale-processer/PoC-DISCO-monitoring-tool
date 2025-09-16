using Microsoft.AspNetCore.Mvc;
using PoC_DISCO_Backend.Dtos;
using PoC_DISCO_Backend.Services;


namespace PoC_DISCO_Backend.Controllers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var loginResult = await _authService.LoginAsync(loginDto);

        if(loginResult is null)
        {
            return Unauthorized("Your credentials are invalid. Please try again.");
        }

        return Ok(loginResult);
    }
}