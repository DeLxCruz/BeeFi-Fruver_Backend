using Domain.Abstractions;

namespace Domain.Entities;

public class CartItem : Entity
{
    public Guid UserId { get; private set; }
    public Guid FruverProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime AddedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual FruverProduct FruverProduct { get; private set; } = null!;

    private CartItem() { }

    private CartItem(Guid id) : base(id) { }

    public static CartItem Create(Guid userId, Guid fruverProductId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(quantity));

        return new CartItem(Guid.NewGuid())
        {
            UserId = userId,
            FruverProductId = fruverProductId,
            Quantity = quantity,
            AddedAt = DateTime.UtcNow
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(quantity));

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
