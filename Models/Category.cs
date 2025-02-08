using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models;

public class Category
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    // Navigation property
    [Required]
    public List<Product>? Products { get; set; }
}
