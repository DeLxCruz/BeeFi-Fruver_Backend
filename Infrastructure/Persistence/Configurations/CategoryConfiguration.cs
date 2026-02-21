using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.IconUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.DisplayOrder)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasIndex(c => new { c.IsActive, c.DisplayOrder });

        // Self-referencing relationship
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data
        var frutasId = Guid.NewGuid();
        var verdurasId = Guid.NewGuid();
        var lacteosId = Guid.NewGuid();
        var carnesId = Guid.NewGuid();

        builder.HasData(
            new { Id = frutasId, Name = "Frutas", Description = "Frutas frescas", IconUrl = "/icons/fruits.png", IsActive = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new { Id = verdurasId, Name = "Verduras", Description = "Verduras frescas", IconUrl = "/icons/vegetables.png", IsActive = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new { Id = lacteosId, Name = "Lácteos", Description = "Productos lácteos", IconUrl = "/icons/dairy.png", IsActive = true, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new { Id = carnesId, Name = "Carnes", Description = "Carnes y embutidos", IconUrl = "/icons/meat.png", IsActive = true, DisplayOrder = 4, CreatedAt = DateTime.UtcNow }
        );
    }
}