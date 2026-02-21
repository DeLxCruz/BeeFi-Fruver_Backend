using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeliveryPersonZoneConfiguration : IEntityTypeConfiguration<DeliveryPersonZone>
{
    public void Configure(EntityTypeBuilder<DeliveryPersonZone> builder)
    {
        builder.ToTable("DeliveryPersonZones");

        builder.HasKey(dpz => dpz.Id);

        builder.Property(dpz => dpz.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(dpz => dpz.AssignedAt)
            .IsRequired();

        // Índice único compuesto: un repartidor no puede ser asignado dos veces a la misma zona
        builder.HasIndex(dpz => new { dpz.DeliveryPersonId, dpz.ZoneId })
            .IsUnique();

        builder.HasIndex(dpz => dpz.DeliveryPersonId);
        builder.HasIndex(dpz => dpz.ZoneId);

        builder.HasOne(dpz => dpz.DeliveryPerson)
            .WithMany(u => u.DeliveryZones)
            .HasForeignKey(dpz => dpz.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dpz => dpz.Zone)
            .WithMany()
            .HasForeignKey(dpz => dpz.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
