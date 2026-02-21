using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Label)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Street)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.HouseNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.AdditionalInfo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Latitude)
            .HasPrecision(10, 7);

        builder.Property(a => a.Longitude)
            .HasPrecision(10, 7);

        builder.Property(a => a.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.HasIndex(a => new { a.UserId, a.IsDefault });

        builder.HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId);

        builder.HasOne(a => a.Zone)
            .WithMany(z => z.Addresses)
            .HasForeignKey(a => a.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}