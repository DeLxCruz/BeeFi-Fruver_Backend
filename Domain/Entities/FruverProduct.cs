using Domain.Abstractions;

namespace Domain.Entities;

public class FruverProduct : Entity, IAuditableEntity
{
    public Guid FruverId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsAvailable { get; private set; }
    public decimal? DiscountPercentage { get; private set; }
    public decimal? BeeFiExclusiveDiscount { get; private set; }
    public bool IsFeatured { get; private set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual User Fruver { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    private FruverProduct() { }

    private FruverProduct(Guid id) : base(id) { }

    public static FruverProduct Create(
        Guid fruverId,
        Guid productId,
        decimal price,
        int stock)
    {
        return new FruverProduct(Guid.NewGuid())
        {
            FruverId = fruverId,
            ProductId = productId,
            Price = price,
            Stock = stock,
            IsAvailable = true,
            IsFeatured = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        decimal price,
        int stock,
        decimal? discountPercentage,
        decimal? beeFiExclusiveDiscount,
        bool isAvailable)
    {
        Price = price;
        Stock = stock;
        DiscountPercentage = discountPercentage;
        BeeFiExclusiveDiscount = beeFiExclusiveDiscount;
        IsAvailable = isAvailable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal price)
    {
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStock(int stock)
    {
        Stock = stock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReduceStock(int quantity)
    {
        if (Stock < quantity)
            throw new InvalidOperationException($"Stock insuficiente. Disponible: {Stock}, Solicitado: {quantity}");

        Stock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RestoreStock(int quantity)
    {
        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDiscount(decimal? discountPercentage)
    {
        DiscountPercentage = discountPercentage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBeeFiDiscount(decimal? beeFiExclusiveDiscount)
    {
        BeeFiExclusiveDiscount = beeFiExclusiveDiscount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFeatured()
    {
        IsFeatured = true;
    }

    public void UnmarkAsFeatured()
    {
        IsFeatured = false;
    }

    public void MakeAvailable()
    {
        IsAvailable = true;
    }

    public void MakeUnavailable()
    {
        IsAvailable = false;
    }

    public decimal GetFinalPrice(bool isBeeFiCustomer = false)
    {
        var price = Price;

        // Aplicar descuento regular
        if (DiscountPercentage.HasValue)
        {
            price -= price * (DiscountPercentage.Value / 100);
        }

        // Aplicar descuento BeeFi si aplica
        if (isBeeFiCustomer && BeeFiExclusiveDiscount.HasValue)
        {
            price -= price * (BeeFiExclusiveDiscount.Value / 100);
        }

        return price;
    }
}