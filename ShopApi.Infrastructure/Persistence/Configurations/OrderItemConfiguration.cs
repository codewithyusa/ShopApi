using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        // Price is snapshotted at time of purchase — it must NOT change if
        // the product's price changes later. That's why OrderItem has its
        // own Price column instead of reading Product.Price at query time.
        builder.Property(oi => oi.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // don't let a product delete wipe out order history
    }
}