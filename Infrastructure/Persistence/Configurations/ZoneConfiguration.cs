using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("Zones");

        builder.HasKey(z => z.Id);

        builder.Property(z => z.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(z => z.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(z => z.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(z => z.DeliveryBaseCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(z => z.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(z => z.CreatedAt)
            .IsRequired();

        builder.HasIndex(z => new { z.City, z.Name });
        builder.HasIndex(z => z.IsActive);
    }
}