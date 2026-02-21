using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserRewardConfiguration : IEntityTypeConfiguration<UserReward>
{
    public void Configure(EntityTypeBuilder<UserReward> builder)
    {
        builder.ToTable("UserRewards");

        builder.HasKey(ur => ur.Id);

        builder.Property(ur => ur.RedeemedAt)
            .IsRequired();

        builder.Property(ur => ur.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasIndex(ur => new { ur.UserId, ur.Status });
        builder.HasIndex(ur => new { ur.RewardId, ur.RedeemedAt });

        builder.HasOne(ur => ur.User)
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ur => ur.Reward)
            .WithMany(r => r.UserRewards)
            .HasForeignKey(ur => ur.RewardId);

        builder.HasOne(ur => ur.Order)
            .WithMany()
            .HasForeignKey(ur => ur.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}