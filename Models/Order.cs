using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Models;

public class Order
{
    [Key]
    [Required]
    public int Id { get; set; }
    public string? UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem>? OrderItems { get; set; }
}

public class OrderItem
{
    [Key]
    [Required]
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Product? Product { get; set; }
}
