using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Seed data
        builder.HasData(
            new { Id = Guid.NewGuid(), Name = "Cliente", Description = "Usuario final que compra productos", CreatedAt = DateTime.UtcNow },
            new { Id = Guid.NewGuid(), Name = "FruverAliado", Description = "Vendedor que publica y gestiona productos", CreatedAt = DateTime.UtcNow },
            new { Id = Guid.NewGuid(), Name = "Empleado", Description = "Personal de entregas y logística", CreatedAt = DateTime.UtcNow },
            new { Id = Guid.NewGuid(), Name = "Administrador", Description = "Gestión completa del sistema", CreatedAt = DateTime.UtcNow }
        );
    }
}