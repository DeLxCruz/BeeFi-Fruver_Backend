using Domain.Abstractions;

namespace Domain.Entities;

public class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public Guid FruverProductId { get; private set; }
    public Guid FruverId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }

    // Snapshot para historial
    public string ProductName { get; private set; } = null!;
    public string? ProductImageUrl { get; private set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual FruverProduct FruverProduct { get; set; } = null!;
    public virtual User Fruver { get; private set; } = null!;

    private OrderItem() { }

    private OrderItem(Guid id) : base(id) { }

    public static OrderItem Create(
        Guid orderId,
        Guid fruverProductId,
        Guid fruverId,
        int quantity,
        decimal unitPrice,
        string productName,
        string? productImageUrl = null)
    {
        return new OrderItem(Guid.NewGuid())
        {
            OrderId = orderId,
            FruverProductId = fruverProductId,
            FruverId = fruverId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Subtotal = unitPrice * quantity,
            ProductName = productName,
            ProductImageUrl = productImageUrl
        };
    }
}