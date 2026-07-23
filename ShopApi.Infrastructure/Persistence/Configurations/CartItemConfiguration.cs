using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Quantity)
            .IsRequired();

        builder.HasOne(c => c.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // if a product is deleted, remove it from carts

        // A user should not have two separate rows for the same product
        // in their cart — quantity should just increment on the existing row.
        builder.HasIndex(c => new { c.UserId, c.ProductId })
            .IsUnique();
    }
}