using Domain.Abstractions;

namespace Domain.Entities;

public class ProductVariant : Entity
{
    public Guid FruverProductId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? SKU { get; private set; }
    public decimal PriceAdjustment { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    // Navigation
    public virtual FruverProduct FruverProduct { get; set; } = null!;

    private ProductVariant() { }

    private ProductVariant(Guid id) : base(id) { }

    public static ProductVariant Create(
        Guid fruverProductId,
        string name,
        decimal priceAdjustment,
        int stock,
        int displayOrder,
        string? sku = null)
    {
        return new ProductVariant(Guid.NewGuid())
        {
            FruverProductId = fruverProductId,
            Name = name,
            SKU = sku,
            PriceAdjustment = priceAdjustment,
            Stock = stock,
            IsActive = true,
            DisplayOrder = displayOrder
        };
    }

    public void UpdateStock(int quantity) => Stock = quantity;

    public decimal GetFinalPrice(decimal basePrice) => basePrice + PriceAdjustment;

    public void Update(
        string name,
        decimal priceAdjustment,
        int stock,
        bool isActive,
        int displayOrder,
        string? sku)
    {
        Name = name;
        SKU = sku;
        PriceAdjustment = priceAdjustment;
        Stock = stock;
        IsActive = isActive;
        DisplayOrder = displayOrder;
    }

    public void Deactivate() => IsActive = false;
}
