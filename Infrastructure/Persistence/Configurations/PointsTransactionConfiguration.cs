using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PointsTransactionConfiguration : IEntityTypeConfiguration<PointsTransaction>
{
    public void Configure(EntityTypeBuilder<PointsTransaction> builder)
    {
        builder.ToTable("PointsTransactions");

        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Points)
            .IsRequired();

        builder.Property(pt => pt.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(pt => pt.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pt => pt.IsBeeFiBonus)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pt => pt.CreatedAt)
            .IsRequired();

        builder.HasIndex(pt => new { pt.UserId, pt.CreatedAt });
        builder.HasIndex(pt => pt.OrderId);

        builder.HasOne(pt => pt.User)
            .WithMany()
            .HasForeignKey(pt => pt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pt => pt.Order)
            .WithMany(o => o.PointsTransactions)
            .HasForeignKey(pt => pt.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}