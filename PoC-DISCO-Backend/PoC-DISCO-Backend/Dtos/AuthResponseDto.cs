namespace PoC_DISCO_Backend.Dtos;

public class AuthResponseDto
{
    public string Token { get; set; }
    public UserResponseDto User { get; set; }
}