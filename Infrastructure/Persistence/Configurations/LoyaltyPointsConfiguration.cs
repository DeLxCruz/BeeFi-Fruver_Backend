using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class LoyaltyPointsConfiguration : IEntityTypeConfiguration<LoyaltyPoints>
{
    public void Configure(EntityTypeBuilder<LoyaltyPoints> builder)
    {
        builder.ToTable("LoyaltyPoints");

        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.TotalPoints)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(lp => lp.AvailablePoints)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(lp => lp.RedeemedPoints)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(lp => lp.CurrentMultiplier)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(lp => lp.LastUpdated)
            .IsRequired();

        builder.HasIndex(lp => lp.UserId)
            .IsUnique();

        builder.HasOne(lp => lp.User)
            .WithOne(u => u.LoyaltyPoints)
            .HasForeignKey<LoyaltyPoints>(lp => lp.UserId);

        builder.HasMany(lp => lp.Transactions)
            .WithOne()
            .HasForeignKey(pt => pt.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}