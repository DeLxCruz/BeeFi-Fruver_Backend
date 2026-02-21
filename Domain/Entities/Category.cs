using Domain.Abstractions;

namespace Domain.Entities;

public class Category : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string IconUrl { get; private set; } = null!;
    public Guid? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; private set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    private Category(Guid id) : base(id) { }

    public static Category Create(
        string name,
        string description,
        string iconUrl,
        int displayOrder,
        Guid? parentCategoryId = null)
    {
        return new Category(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            IconUrl = iconUrl,
            ParentCategoryId = parentCategoryId,
            IsActive = true,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, string iconUrl, int displayOrder)
    {
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        DisplayOrder = displayOrder;
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