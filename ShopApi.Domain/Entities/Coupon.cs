namespace ShopApi.Domain.Entities;

public class Coupon
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required decimal DiscountPercentage { get; set; }
    public required DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}