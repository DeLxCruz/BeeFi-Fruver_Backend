using Domain.Abstractions;

namespace Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    private RefreshToken() { }

    private RefreshToken(Guid id) : base(id) { }

    public static RefreshToken Create(
        Guid userId,
        string token,
        int expiryDays,
        string? deviceInfo = null,
        string? ipAddress = null)
    {
        return new RefreshToken(Guid.NewGuid())
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress
        };
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}