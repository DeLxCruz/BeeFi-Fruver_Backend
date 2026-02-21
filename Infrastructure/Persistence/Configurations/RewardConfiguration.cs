using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RewardConfiguration : IEntityTypeConfiguration<Reward>
{
    public void Configure(EntityTypeBuilder<Reward> builder)
    {
        builder.ToTable("Rewards");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.ImageUrl)
            .HasMaxLength(500);

        builder.Property(r => r.PointsRequired)
            .IsRequired();

        builder.Property(r => r.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.Value)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.IsBeeFiExclusive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.MaxRedemptionsPerUser)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => new { r.IsActive, r.PointsRequired });
        builder.HasIndex(r => r.IsBeeFiExclusive);

        builder.HasMany(r => r.UserRewards)
            .WithOne(ur => ur.Reward)
            .HasForeignKey(ur => ur.RewardId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}