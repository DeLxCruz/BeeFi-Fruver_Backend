using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("Banners");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.LinkUrl)
            .HasMaxLength(500);

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.DisplayOrder)
            .IsRequired();

        builder.Property(b => b.StartsAt);

        builder.Property(b => b.EndsAt);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.CreatedBy);

        // Índice para consultas filtradas por estado y orden de visualización
        builder.HasIndex(b => new { b.IsActive, b.DisplayOrder });
    }
}
