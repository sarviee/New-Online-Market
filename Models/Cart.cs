using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models;

public class CartItem
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public Product? Product { get; set; }
}
