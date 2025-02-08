using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models;

public class ResetPasswordViewModel
{

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
}
