using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BeeFiPlanConfiguration : IEntityTypeConfiguration<BeeFiPlan>
{
    public void Configure(EntityTypeBuilder<BeeFiPlan> builder)
    {
        builder.ToTable("BeeFiPlans");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.MonthlyPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.SpeedMbps)
            .IsRequired();

        builder.Property(p => p.DiscountPercentage)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(p => p.BonusPointsMultiplier)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // Seed data
        var basicPlanId = Guid.NewGuid();
        var plusPlanId = Guid.NewGuid();
        var premiumPlanId = Guid.NewGuid();

        builder.HasData(
            new
            {
                Id = basicPlanId,
                Name = "Básico",
                Description = "Plan básico de internet con beneficios en BeeFi",
                MonthlyPrice = 50000m,
                SpeedMbps = 50,
                DiscountPercentage = 5m,
                BonusPointsMultiplier = 1,
                HasFreeDelivery = true,
                FreeDeliveriesPerMonth = 1,
                HasPrioritySupport = false,
                HasEarlyAccess = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                Id = plusPlanId,
                Name = "Plus",
                Description = "Plan plus con más beneficios",
                MonthlyPrice = 80000m,
                SpeedMbps = 100,
                DiscountPercentage = 10m,
                BonusPointsMultiplier = 2,
                HasFreeDelivery = true,
                FreeDeliveriesPerMonth = 3,
                HasPrioritySupport = true,
                HasEarlyAccess = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new
            {
                Id = premiumPlanId,
                Name = "Premium",
                Description = "Plan premium con todos los beneficios",
                MonthlyPrice = 120000m,
                SpeedMbps = 200,
                DiscountPercentage = 15m,
                BonusPointsMultiplier = 3,
                HasFreeDelivery = true,
                FreeDeliveriesPerMonth = 5,
                HasPrioritySupport = true,
                HasEarlyAccess = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}