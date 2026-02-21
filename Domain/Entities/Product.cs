using Domain.Abstractions;

namespace Domain.Entities;

public class Product : Entity, IAuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public string MainImageUrl { get; private set; } = null!;
    public string UnitOfMeasure { get; private set; } = null!; // kg, unidad, lb, paquete
    public bool IsActive { get; private set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();
    public virtual ICollection<FruverProduct> FruverProducts { get; private set; } = new List<FruverProduct>();

    private Product() { }

    private Product(Guid id) : base(id) { }

    public static Product Create(
        string name,
        string description,
        Guid categoryId,
        string mainImageUrl,
        string unitOfMeasure)
    {
        return new Product(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            CategoryId = categoryId,
            MainImageUrl = mainImageUrl,
            UnitOfMeasure = unitOfMeasure,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, Guid categoryId, string unitOfMeasure)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UnitOfMeasure = unitOfMeasure;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMainImage(string imageUrl)
    {
        MainImageUrl = imageUrl;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}