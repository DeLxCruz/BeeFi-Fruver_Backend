using Domain.Abstractions;

namespace Domain.Entities;

public class Banner : Entity
{
    public string Title { get; private set; } = null!;
    public string ImageUrl { get; private set; } = null!;
    public string? LinkUrl { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime? StartsAt { get; private set; }
    public DateTime? EndsAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    private Banner() { }

    private Banner(Guid id) : base(id) { }

    public static Banner Create(string title, string imageUrl, int displayOrder,
        string? linkUrl = null, DateTime? startsAt = null, DateTime? endsAt = null,
        Guid? createdBy = null)
    {
        return new Banner(Guid.NewGuid())
        {
            Title = title,
            ImageUrl = imageUrl,
            LinkUrl = linkUrl,
            IsActive = true,
            DisplayOrder = displayOrder,
            StartsAt = startsAt,
            EndsAt = endsAt,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string title, string imageUrl, string? linkUrl, int displayOrder,
        DateTime? startsAt, DateTime? endsAt)
    {
        Title = title;
        ImageUrl = imageUrl;
        LinkUrl = linkUrl;
        DisplayOrder = displayOrder;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public bool IsCurrentlyActive()
    {
        var now = DateTime.UtcNow;
        return IsActive
               && (StartsAt == null || StartsAt <= now)
               && (EndsAt == null || EndsAt >= now);
    }
}
