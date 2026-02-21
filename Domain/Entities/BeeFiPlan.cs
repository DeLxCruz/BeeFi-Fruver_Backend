using Domain.Abstractions;

namespace Domain.Entities;

public class BeeFiPlan : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal MonthlyPrice { get; private set; }
    public int SpeedMbps { get; private set; }

    // Beneficios en app de fruvers
    public decimal DiscountPercentage { get; private set; }
    public int BonusPointsMultiplier { get; private set; }
    public bool HasFreeDelivery { get; private set; }
    public int FreeDeliveriesPerMonth { get; private set; }
    public bool HasPrioritySupport { get; private set; }
    public bool HasEarlyAccess { get; private set; }

    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual ICollection<BeeFiSubscription> Subscriptions { get; private set; } = new List<BeeFiSubscription>();
    public virtual ICollection<BeeFiBenefit> Benefits { get; private set; } = new List<BeeFiBenefit>();

    private BeeFiPlan() { }

    private BeeFiPlan(Guid id) : base(id) { }

    public static BeeFiPlan Create(
        string name,
        string description,
        decimal monthlyPrice,
        int speedMbps,
        decimal discountPercentage,
        int bonusPointsMultiplier,
        int freeDeliveriesPerMonth)
    {
        return new BeeFiPlan(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            MonthlyPrice = monthlyPrice,
            SpeedMbps = speedMbps,
            DiscountPercentage = discountPercentage,
            BonusPointsMultiplier = bonusPointsMultiplier,
            HasFreeDelivery = freeDeliveriesPerMonth > 0,
            FreeDeliveriesPerMonth = freeDeliveriesPerMonth,
            HasPrioritySupport = false,
            HasEarlyAccess = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateBenefits(
        decimal discountPercentage,
        int bonusPointsMultiplier,
        int freeDeliveriesPerMonth)
    {
        DiscountPercentage = discountPercentage;
        BonusPointsMultiplier = bonusPointsMultiplier;
        FreeDeliveriesPerMonth = freeDeliveriesPerMonth;
        HasFreeDelivery = freeDeliveriesPerMonth > 0;
    }

    public void EnablePremiumFeatures()
    {
        HasPrioritySupport = true;
        HasEarlyAccess = true;
    }
}