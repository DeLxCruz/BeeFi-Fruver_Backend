using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SalesAggDailyConfiguration : IEntityTypeConfiguration<SalesAggDaily>
{
    public void Configure(EntityTypeBuilder<SalesAggDaily> builder)
    {
        builder.ToTable("SalesAggDaily");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProductKey)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Revenue)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.UnitsSold)
            .IsRequired();

        builder.Property(s => s.OrderCount)
            .IsRequired();

        builder.HasIndex(s => new { s.ProductKey, s.Date, s.ZoneId })
            .IsUnique();

        builder.HasIndex(s => new { s.ProductKey, s.ZoneId });
    }
}
