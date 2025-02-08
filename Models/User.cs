using Microsoft.AspNetCore.Identity;

namespace OnlineMarket.Models;
public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
