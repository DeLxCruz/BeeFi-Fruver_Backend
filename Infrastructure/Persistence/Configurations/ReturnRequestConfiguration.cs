using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
{
    public void Configure(EntityTypeBuilder<ReturnRequest> builder)
    {
        builder.ToTable("ReturnRequests");

        builder.HasKey(rr => rr.Id);

        builder.Property(rr => rr.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(rr => rr.EvidenceUrl)
            .HasMaxLength(500);

        builder.Property(rr => rr.AdminNotes)
            .HasMaxLength(1000);

        builder.Property(rr => rr.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(rr => rr.RefundType)
            .HasConversion<string>();

        builder.Property(rr => rr.RefundAmount)
            .HasPrecision(18, 2);

        builder.Property(rr => rr.CreatedAt)
            .IsRequired();

        // One return request per order
        builder.HasIndex(rr => rr.OrderId)
            .IsUnique();

        builder.HasIndex(rr => new { rr.UserId, rr.Status });

        builder.HasOne(rr => rr.Order)
            .WithMany()
            .HasForeignKey(rr => rr.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rr => rr.User)
            .WithMany()
            .HasForeignKey(rr => rr.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
