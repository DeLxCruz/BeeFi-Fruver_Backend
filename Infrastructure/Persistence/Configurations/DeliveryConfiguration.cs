using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("Deliveries");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.TrackingNotes)
            .HasMaxLength(1000);

        builder.Property(d => d.DeliveryMode)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.DeliveryMode.BeeFiLogistics);

        builder.Property(d => d.SellerDeliveryFee)
            .HasPrecision(18, 2);

        builder.Property(d => d.DeliveryProofUrl)
            .HasMaxLength(500);

        builder.Property(d => d.DeliveryPin)
            .HasMaxLength(10);

        builder.Property(d => d.SellerDeliveryPersonName)
            .HasMaxLength(200);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.HasIndex(d => new { d.DeliveryPersonId, d.Status });
        builder.HasIndex(d => d.Status);

        builder.HasOne(d => d.Order)
            .WithOne(o => o.Delivery)
            .HasForeignKey<Delivery>(d => d.OrderId);

        builder.HasOne(d => d.DeliveryPerson)
            .WithMany()
            .HasForeignKey(d => d.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.StatusHistory)
            .WithOne(dsh => dsh.Delivery)
            .HasForeignKey(dsh => dsh.DeliveryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}