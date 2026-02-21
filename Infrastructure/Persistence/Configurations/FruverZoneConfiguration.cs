using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FruverZoneConfiguration : IEntityTypeConfiguration<FruverZone>
{
    public void Configure(EntityTypeBuilder<FruverZone> builder)
    {
        builder.ToTable("FruverZones");

        builder.HasKey(fz => new { fz.FruverId, fz.ZoneId });

        builder.Property(fz => fz.AssignedAt)
            .IsRequired();

        builder.HasOne(fz => fz.Fruver)
            .WithMany(u => u.FruverZones)
            .HasForeignKey(fz => fz.FruverId);

        builder.HasOne(fz => fz.Zone)
            .WithMany(z => z.FruverZones)
            .HasForeignKey(fz => fz.ZoneId);
    }
}