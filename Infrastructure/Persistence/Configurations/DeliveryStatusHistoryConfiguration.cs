using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeliveryStatusHistoryConfiguration : IEntityTypeConfiguration<DeliveryStatusHistory>
{
    public void Configure(EntityTypeBuilder<DeliveryStatusHistory> builder)
    {
        builder.ToTable("DeliveryStatusHistory");

        builder.HasKey(dsh => dsh.Id);

        builder.Property(dsh => dsh.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(dsh => dsh.Timestamp)
            .IsRequired();

        builder.Property(dsh => dsh.Latitude)
            .HasPrecision(10, 7);

        builder.Property(dsh => dsh.Longitude)
            .HasPrecision(10, 7);

        builder.Property(dsh => dsh.Notes)
            .HasMaxLength(500);

        builder.HasIndex(dsh => new { dsh.DeliveryId, dsh.Timestamp });

        builder.HasOne(dsh => dsh.Delivery)
            .WithMany(d => d.StatusHistory)
            .HasForeignKey(dsh => dsh.DeliveryId);

        builder.HasOne(dsh => dsh.UpdatedByUser)
            .WithMany()
            .HasForeignKey(dsh => dsh.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}