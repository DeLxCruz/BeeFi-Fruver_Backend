using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class UserReward : Entity
{
    public Guid UserId { get; private set; }
    public Guid RewardId { get; private set; }
    public Guid? OrderId { get; private set; }
    public DateTime RedeemedAt { get; private set; }
    public RewardStatus Status { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public DateTime? UsedAt { get; private set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Reward Reward { get; set; } = null!;
    public virtual Order? Order { get; set; }

    private UserReward() { }

    private UserReward(Guid id) : base(id) { }

    public static UserReward Create(
        Guid userId,
        Guid rewardId,
        DateTime? expirationDate = null)
    {
        return new UserReward(Guid.NewGuid())
        {
            UserId = userId,
            RewardId = rewardId,
            RedeemedAt = DateTime.UtcNow,
            Status = RewardStatus.Active,
            ExpirationDate = expirationDate ?? DateTime.UtcNow.AddDays(30)
        };
    }

    public void Use(Guid orderId)
    {
        if (Status != RewardStatus.Active)
            throw new InvalidOperationException("La recompensa ya fue usada o expiró");

        if (ExpirationDate.HasValue && DateTime.UtcNow > ExpirationDate)
            throw new InvalidOperationException("La recompensa ha expirado");

        Status = RewardStatus.Used;
        OrderId = orderId;
        UsedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        Status = RewardStatus.Expired;
    }

    public bool IsValid()
    {
        return Status == RewardStatus.Active &&
               (!ExpirationDate.HasValue || DateTime.UtcNow <= ExpirationDate);
    }
}