using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(u => u.PhoneNumber);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.EmailConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.PhoneConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        // Account approval properties
        builder.Property(u => u.AccountStatus)
            .IsRequired()
            .HasDefaultValue(AccountStatus.Approved)
            .HasConversion<int>();

        builder.Property(u => u.RejectionReason)
            .HasMaxLength(500);

        builder.HasIndex(u => u.AccountStatus);

        // Auditable properties
        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100);

        builder.Property(u => u.UpdatedBy)
            .HasMaxLength(100);

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.BeeFiSubscription)
            .WithOne(s => s.User)
            .HasForeignKey<BeeFiSubscription>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.LoyaltyPoints)
            .WithOne(lp => lp.User)
            .HasForeignKey<LoyaltyPoints>(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.FruverProducts)
            .WithOne(fp => fp.Fruver)
            .HasForeignKey(fp => fp.FruverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.DeviceTokens)
            .WithOne(dt => dt.User)
            .HasForeignKey(dt => dt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.DeliveryZones)
            .WithOne(dpz => dpz.DeliveryPerson)
            .HasForeignKey(dpz => dpz.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}