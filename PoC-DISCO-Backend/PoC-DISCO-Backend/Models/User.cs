using System.ComponentModel.DataAnnotations;

namespace PoC_DISCO_Backend.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}