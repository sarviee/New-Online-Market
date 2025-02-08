
using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models;
public class LoginViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
