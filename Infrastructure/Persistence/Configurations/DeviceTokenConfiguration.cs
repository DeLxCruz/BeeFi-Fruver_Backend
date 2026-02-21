using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.ToTable("DeviceTokens");

        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(dt => dt.Token)
            .IsUnique();

        builder.Property(dt => dt.Platform)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(dt => dt.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(dt => dt.CreatedAt)
            .IsRequired();

        builder.HasIndex(dt => new { dt.UserId, dt.IsActive });

        builder.HasOne(dt => dt.User)
            .WithMany(u => u.DeviceTokens)
            .HasForeignKey(dt => dt.UserId);
    }
}