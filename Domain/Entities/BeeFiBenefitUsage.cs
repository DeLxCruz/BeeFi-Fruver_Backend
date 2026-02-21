using Domain.Abstractions;

namespace Domain.Entities;

public class BeeFiBenefitUsage : Entity
{
    public Guid UserId { get; private set; }
    public Guid BenefitId { get; private set; }
    public Guid? OrderId { get; private set; }
    public decimal ValueApplied { get; private set; }
    public DateTime UsedAt { get; private set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual BeeFiBenefit Benefit { get; set; } = null!;
    public virtual Order? Order { get; set; }

    private BeeFiBenefitUsage() { }

    private BeeFiBenefitUsage(Guid id) : base(id) { }

    public static BeeFiBenefitUsage Create(
        Guid userId,
        Guid benefitId,
        decimal valueApplied,
        Guid? orderId = null)
    {
        return new BeeFiBenefitUsage(Guid.NewGuid())
        {
            UserId = userId,
            BenefitId = benefitId,
            OrderId = orderId,
            ValueApplied = valueApplied,
            UsedAt = DateTime.UtcNow
        };
    }
}