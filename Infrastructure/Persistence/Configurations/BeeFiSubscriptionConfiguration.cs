using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BeeFiSubscriptionConfiguration : IEntityTypeConfiguration<BeeFiSubscription>
{
    public void Configure(EntityTypeBuilder<BeeFiSubscription> builder)
    {
        builder.ToTable("BeeFiSubscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ContractNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.ContractNumber)
            .IsUnique();

        builder.Property(s => s.BeeFiCustomerId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.BeeFiCustomerId);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.IsVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => new { s.UserId, s.Status });

        builder.HasOne(s => s.User)
            .WithOne(u => u.BeeFiSubscription)
            .HasForeignKey<BeeFiSubscription>(s => s.UserId);

        builder.HasOne(s => s.Plan)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}