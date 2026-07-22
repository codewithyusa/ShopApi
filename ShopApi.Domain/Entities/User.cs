namespace ShopApi.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? Phone { get; set; }
    public string Role { get; set; } = "customer";

    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordExpires { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}