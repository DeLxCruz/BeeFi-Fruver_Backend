using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Notification : Entity
{
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? Data { get; private set; } // JSON
    public bool IsRead { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime? SentAt { get; private set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    private Notification() { }

    private Notification(Guid id) : base(id) { }

    public static Notification Create(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string? data = null)
    {
        return new Notification(Guid.NewGuid())
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Data = data,
            IsRead = false,
            IsSent = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsSent()
    {
        IsSent = true;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}