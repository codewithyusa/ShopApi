using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        // Without this, EF/Npgsql picks a default precision that can
        // silently round or truncate prices. Always be explicit with money.
        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Image)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Size)
            .IsRequired()
            .HasMaxLength(50);

        // Index for filtering/sorting by category — you'll be doing this
        // constantly on a shop front page.
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsFeatured);
    }
}