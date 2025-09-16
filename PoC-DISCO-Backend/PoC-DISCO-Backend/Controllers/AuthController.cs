using Microsoft.AspNetCore.Authorization;
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
    
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] TokenDto tokenDto)
    {
        try
        {
            var authResponse = await _authService.GetRefreshTokenAsync(tokenDto);
            
            if (authResponse is null) return BadRequest("Invalid client request");
        
            return Ok(authResponse);
        }
        catch (Exception _)
        {
            return BadRequest("Invalid client request");
        }
    }
    
    [HttpPost("revoke"), Authorize]
    public async Task<ActionResult> Revoke()
    {
        var userName = User.Identity?.Name;
        var user = await _authService.RevokeRefreshTokenAsync(userName);
        if (user is null) return BadRequest("Invalid client request");
        return NoContent();
    }
}