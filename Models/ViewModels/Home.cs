
using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models;
public class HomeViewModel
{
    [Required]
    public List<Product> Products { get; set; } = [];

    [Required]
    public List<Category> Categories { get; set; } = [];

    public int? CategoryId { get; set; }
}
