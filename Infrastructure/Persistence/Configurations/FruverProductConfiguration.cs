using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FruverProductConfiguration : IEntityTypeConfiguration<FruverProduct>
{
    public void Configure(EntityTypeBuilder<FruverProduct> builder)
    {
        builder.ToTable("FruverProducts");

        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(fp => fp.Stock)
            .IsRequired();

        builder.Property(fp => fp.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(fp => fp.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(fp => fp.BeeFiExclusiveDiscount)
            .HasPrecision(5, 2);

        builder.Property(fp => fp.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false);

        // PASO 6: campos adicionales
        builder.Property(fp => fp.PreparationTimeMinutes)
            .IsRequired()
            .HasDefaultValue(30);

        builder.Property(fp => fp.IsSeasonal)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(fp => fp.AllowPreOrder)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(fp => fp.CreatedAt)
            .IsRequired();

        builder.HasIndex(fp => new { fp.FruverId, fp.IsAvailable });
        builder.HasIndex(fp => new { fp.ProductId, fp.FruverId })
            .IsUnique();

        builder.HasOne(fp => fp.Fruver)
            .WithMany(u => u.FruverProducts)
            .HasForeignKey(fp => fp.FruverId);

        builder.HasOne(fp => fp.Product)
            .WithMany(p => p.FruverProducts)
            .HasForeignKey(fp => fp.ProductId);
    }
}