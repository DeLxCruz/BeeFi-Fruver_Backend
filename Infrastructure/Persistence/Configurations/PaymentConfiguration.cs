using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Method)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.TransactionId)
            .HasMaxLength(200);

        builder.HasIndex(p => p.TransactionId);

        builder.Property(p => p.GatewayResponse)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.RefundAmount)
            .HasPrecision(18, 2);

        builder.Property(p => p.RefundedAt);

        builder.Property(p => p.RefundReason)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasIndex(p => new { p.OrderId, p.Status });

        builder.HasOne(p => p.Order)
            .WithOne(o => o.Payment)
            .HasForeignKey<Payment>(p => p.OrderId);
    }
}