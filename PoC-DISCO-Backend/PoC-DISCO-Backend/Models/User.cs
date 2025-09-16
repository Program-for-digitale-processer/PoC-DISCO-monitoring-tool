using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PoC_DISCO_Backend;

public class User
{
    public int Id { get; set; }
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
}