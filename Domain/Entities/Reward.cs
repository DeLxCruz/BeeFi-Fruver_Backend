using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class Reward : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public int PointsRequired { get; private set; }
    public RewardType Type { get; private set; }
    public decimal Value { get; private set; }
    public bool IsBeeFiExclusive { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public int MaxRedemptionsPerUser { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual ICollection<UserReward> UserRewards { get; private set; } = new List<UserReward>();

    private Reward() { }

    private Reward(Guid id) : base(id) { }

    public static Reward Create(
        string name,
        string description,
        int pointsRequired,
        RewardType type,
        decimal value,
        bool isBeeFiExclusive = false,
        int maxRedemptionsPerUser = 1,
        DateTime? expirationDate = null,
        string? imageUrl = null)
    {
        return new Reward(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
            PointsRequired = pointsRequired,
            Type = type,
            Value = value,
            IsBeeFiExclusive = isBeeFiExclusive,
            IsActive = true,
            ExpirationDate = expirationDate,
            MaxRedemptionsPerUser = maxRedemptionsPerUser,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsValid()
    {
        return IsActive && (ExpirationDate == null || ExpirationDate > DateTime.UtcNow);
    }

    public void Update(
        string name,
        string description,
        string? imageUrl,
        int pointsRequired,
        decimal value,
        bool isActive,
        int maxRedemptionsPerUser,
        DateTime? expirationDate)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        PointsRequired = pointsRequired;
        Value = value;
        IsActive = isActive;
        MaxRedemptionsPerUser = maxRedemptionsPerUser;
        ExpirationDate = expirationDate;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}