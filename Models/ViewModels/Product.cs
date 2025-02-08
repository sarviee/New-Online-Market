using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models;

public class ProductViewModel
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public int Stock { get; set; }

    public IFormFile? ImageFile { get; set; }
    public string? ImagePath { get; set; }
    [Required]
    public int CategoryId { get; set; } // Selected category
    public List<Category>? Categories { get; set; } // List of all categories
}
