using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PriceReferenceConfiguration : IEntityTypeConfiguration<PriceReference>
{
    public void Configure(EntityTypeBuilder<PriceReference> builder)
    {
        builder.ToTable("PriceReferences");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.ProductKey)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pr => pr.P25)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(pr => pr.P50)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(pr => pr.P75)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(pr => pr.UnitNorm)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pr => pr.SampleCount)
            .IsRequired();

        builder.Property(pr => pr.ComputedAt)
            .IsRequired();

        builder.Property(pr => pr.WindowDays)
            .IsRequired();

        builder.HasIndex(pr => new { pr.ProductKey, pr.ZoneId })
            .IsUnique();

        builder.HasIndex(pr => pr.ProductKey);
    }
}
