using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class DeviceToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DevicePlatform Platform { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastUsed { get; private set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    private DeviceToken() { }

    private DeviceToken(Guid id) : base(id) { }

    public static DeviceToken Create(
        Guid userId,
        string token,
        DevicePlatform platform)
    {
        return new DeviceToken(Guid.NewGuid())
        {
            UserId = userId,
            Token = token,
            Platform = platform,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastUsed = DateTime.UtcNow
        };
    }

    public void UpdateLastUsed()
    {
        LastUsed = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
        LastUsed = DateTime.UtcNow;
    }
}