using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pv => pv.SKU)
            .HasMaxLength(50);

        builder.HasIndex(pv => pv.SKU)
            .IsUnique()
            .HasFilter("[SKU] IS NOT NULL");

        builder.Property(pv => pv.PriceAdjustment)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(pv => pv.Stock)
            .IsRequired();

        builder.Property(pv => pv.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pv => pv.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(pv => new { pv.FruverProductId, pv.IsActive });

        builder.HasOne(pv => pv.FruverProduct)
            .WithMany(fp => fp.Variants)
            .HasForeignKey(pv => pv.FruverProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
