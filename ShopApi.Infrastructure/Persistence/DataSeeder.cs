using Microsoft.EntityFrameworkCore;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(ShopDbContext context, CancellationToken ct = default)
    {
        await context.Database.MigrateAsync(ct);

        if (await context.Products.AnyAsync(ct))
            return; // already seeded

        var products = new List<Product>
        {
            new()
            {
                Name = "Classic T-Shirt", Description = "100% cotton crew neck",
                Price = 19.99m, Image = "https://placehold.co/400x400?text=T-Shirt",
                Category = "Apparel", Color = "Black", Size = "M",
                Stock = 50, IsFeatured = true,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Running Sneakers", Description = "Lightweight everyday trainer",
                Price = 79.99m, Image = "https://placehold.co/400x400?text=Sneakers",
                Category = "Footwear", Color = "White", Size = "42",
                Stock = 30, IsFeatured = true,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Name = "Denim Jacket", Description = "Vintage wash denim",
                Price = 59.99m, Image = "https://placehold.co/400x400?text=Jacket",
                Category = "Apparel", Color = "Blue", Size = "L",
                Stock = 20, IsFeatured = false,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync(ct);
    }
}