using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        builder.Property(ci => ci.AddedAt)
            .IsRequired();

        builder.Property(ci => ci.UpdatedAt);

        // Un usuario no puede tener el mismo FruverProduct dos veces en el carrito
        builder.HasIndex(ci => new { ci.UserId, ci.FruverProductId })
            .IsUnique();

        // Índice para consultas rápidas por usuario
        builder.HasIndex(ci => ci.UserId);

        builder.HasOne(ci => ci.User)
            .WithMany()
            .HasForeignKey(ci => ci.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.FruverProduct)
            .WithMany()
            .HasForeignKey(ci => ci.FruverProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
