using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.PaymentRef)
            .HasMaxLength(255);

        builder.Property(o => o.PaymentStatus)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("pending");

        builder.Property(o => o.OrderStatus)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("pending");

        // An order's line items disappear with the order — but the order
        // itself is never cascade-deleted from User (see UserConfiguration).
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.PaymentStatus);
    }
}