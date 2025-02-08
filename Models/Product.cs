using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarket.Models;

public class Product
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string? ImagePath { get; set; }
    public int Stock { get; set; }

    // Foreign key
    [Required]
    public int CategoryId { get; set; }

    // Navigation property
    public Category? Category { get; set; }

    // Foreign Key Property
    [ForeignKey("Seller")]
    public string SellerId { get; set; } = string.Empty;

    // Navigation Property
    public User? Seller { get; set; }
}


