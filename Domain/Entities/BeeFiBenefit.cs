using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class BeeFiBenefit : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public BenefitType Type { get; private set; }
    public decimal Value { get; private set; }
    public Guid? RequiredPlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public int MaxUsesPerUser { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public virtual BeeFiPlan? RequiredPlan { get; set; }
    public virtual ICollection<BeeFiBenefitUsage> Usages { get; private set; } = new List<BeeFiBenefitUsage>();

    private BeeFiBenefit() { }

    private BeeFiBenefit(Guid id) : base(id) { }

    public static BeeFiBenefit Create(
        string name,
        string description,
        BenefitType type,
        decimal value,
        DateTime startDate,
        DateTime? endDate,
        int maxUsesPerUser = 1,
        Guid? requiredPlanId = null)
    {
        return new BeeFiBenefit(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            Type = type,
            Value = value,
            RequiredPlanId = requiredPlanId,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true,
            MaxUsesPerUser = maxUsesPerUser,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsValidForDate(DateTime date)
    {
        return date >= StartDate && (EndDate == null || date <= EndDate);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}