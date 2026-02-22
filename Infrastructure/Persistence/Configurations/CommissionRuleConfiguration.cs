using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CommissionRuleConfiguration : IEntityTypeConfiguration<CommissionRule>
{
    public void Configure(EntityTypeBuilder<CommissionRule> builder)
    {
        builder.ToTable("CommissionRules");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cr => cr.CommissionType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(cr => cr.CommissionValue)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(cr => cr.MinCommission)
            .HasPrecision(18, 4);

        builder.Property(cr => cr.MaxCommission)
            .HasPrecision(18, 4);

        builder.Property(cr => cr.MinOrderAmount)
            .HasPrecision(18, 2);

        builder.Property(cr => cr.MaxOrderAmount)
            .HasPrecision(18, 2);

        builder.Property(cr => cr.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cr => cr.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(cr => cr.CreatedAt)
            .IsRequired();

        builder.HasIndex(cr => new { cr.IsActive, cr.ValidFrom, cr.ValidTo });

        builder.HasOne(cr => cr.Zone)
            .WithMany()
            .HasForeignKey(cr => cr.ZoneId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(cr => cr.Category)
            .WithMany()
            .HasForeignKey(cr => cr.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(cr => cr.Role)
            .WithMany()
            .HasForeignKey(cr => cr.RoleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
