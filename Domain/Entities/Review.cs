using Domain.Abstractions;

namespace Domain.Entities;

public class Review : Entity, IAuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid FruverId { get; private set; }
    public Guid OrderId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public bool IsVisible { get; private set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual User Fruver { get; private set; } = null!;
    public virtual Order Order { get; private set; } = null!;

    private Review() { }

    private Review(Guid id) : base(id) { }

    public static Review Create(Guid userId, Guid fruverId, Guid orderId, int rating, string? comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "La calificación debe estar entre 1 y 5");

        return new Review(Guid.NewGuid())
        {
            UserId = userId,
            FruverId = fruverId,
            OrderId = orderId,
            Rating = rating,
            Comment = comment,
            IsVisible = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Hide() => IsVisible = false;

    public void Show() => IsVisible = true;

    public void Update(int rating, string? comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "La calificación debe estar entre 1 y 5");

        Rating = rating;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }
}
