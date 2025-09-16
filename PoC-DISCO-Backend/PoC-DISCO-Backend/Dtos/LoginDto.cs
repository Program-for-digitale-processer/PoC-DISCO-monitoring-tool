using System.ComponentModel.DataAnnotations;

namespace PoC_DISCO_Backend.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}