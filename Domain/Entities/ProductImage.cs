using Domain.Abstractions;

namespace Domain.Entities;

public class ProductImage : Entity
{
    public Guid ProductId { get; private set; }
    public string ImageUrl { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public DateTime UploadedAt { get; private set; }

    // Navigation properties
    public virtual Product Product { get; set; } = null!;

    private ProductImage() { }

    private ProductImage(Guid id) : base(id) { }

    public static ProductImage Create(Guid productId, string imageUrl, int displayOrder)
    {
        return new ProductImage(Guid.NewGuid())
        {
            ProductId = productId,
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder,
            UploadedAt = DateTime.UtcNow
        };
    }
}