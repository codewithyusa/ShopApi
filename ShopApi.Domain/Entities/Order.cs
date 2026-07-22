namespace ShopApi.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public required decimal TotalAmount { get; set; }
    public string? PaymentRef { get; set; }
    public string PaymentStatus { get; set; } = "pending";
    public string OrderStatus { get; set; } = "pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}