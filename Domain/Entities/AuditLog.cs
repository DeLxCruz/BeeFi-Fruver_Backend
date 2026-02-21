using Domain.Abstractions;

namespace Domain.Entities;

public class AuditLog : Entity
{
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string Entity { get; private set; } = null!;
    public string EntityId { get; private set; } = null!;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime Timestamp { get; private set; }

    // Navigation properties
    public virtual User? User { get; set; }

    private AuditLog() { }

    private AuditLog(Guid id) : base(id) { }

    public static AuditLog Create(
        string action,
        string entity,
        string entityId,
        Guid? userId = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new AuditLog(Guid.NewGuid())
        {
            UserId = userId,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow
        };
    }
}