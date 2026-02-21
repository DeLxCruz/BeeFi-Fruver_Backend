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

        // Seed data — GUIDs fijos para evitar migraciones fantasma
        builder.HasData(
            new
            {
                Id = new Guid("E93BD156-71F2-4D7E-836B-224752E64A66"),
                Name = "Cliente",
                Description = "Usuario final que compra productos",
                CreatedAt = new DateTime(2026, 2, 21, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = new Guid("A45227E0-CB30-4924-9338-2AD0DE80661C"),
                Name = "Empleado",
                Description = "Personal de entregas y logística",
                CreatedAt = new DateTime(2026, 2, 21, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = new Guid("A20B5B31-2317-4716-8B9E-D7FDCD642EA9"),
                Name = "Administrador",
                Description = "Gestión completa del sistema",
                CreatedAt = new DateTime(2026, 2, 21, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = new Guid("24AF6F6C-1269-449B-9B63-FD6D1E49433A"),
                Name = "FruverAliado",
                Description = "Vendedor que publica y gestiona productos",
                CreatedAt = new DateTime(2026, 2, 21, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}