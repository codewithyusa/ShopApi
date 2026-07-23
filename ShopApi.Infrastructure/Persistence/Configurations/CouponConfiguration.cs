using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.DiscountPercentage)
            .IsRequired()
            .HasPrecision(5, 2); // e.g. 100.00 max, two decimal places

        // Coupon codes must be globally unique so lookups by code are safe.
        builder.HasIndex(c => c.Code)
            .IsUnique();
    }
}