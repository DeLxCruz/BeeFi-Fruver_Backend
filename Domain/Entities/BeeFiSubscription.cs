using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class BeeFiSubscription : Entity, IAuditableEntity
{
    public Guid UserId { get; private set; }
    public string ContractNumber { get; private set; } = null!;
    public string BeeFiCustomerId { get; private set; } = null!;
    public Guid PlanId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    // IAuditableEntity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual BeeFiPlan Plan { get; set; } = null!;

    private BeeFiSubscription() { }

    private BeeFiSubscription(Guid id) : base(id) { }

    public static BeeFiSubscription Create(
        Guid userId,
        string contractNumber,
        string beeFiCustomerId,
        Guid planId)
    {
        return new BeeFiSubscription(Guid.NewGuid())
        {
            UserId = userId,
            ContractNumber = contractNumber,
            BeeFiCustomerId = beeFiCustomerId,
            PlanId = planId,
            Status = SubscriptionStatus.Pending,
            StartDate = DateTime.UtcNow,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Activate()
    {
        Status = SubscriptionStatus.Active;
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
        StartDate = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = SubscriptionStatus.Suspended;
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        EndDate = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        Status = SubscriptionStatus.Active;
        EndDate = null;
    }

    public bool IsActive => Status == SubscriptionStatus.Active && IsVerified;
}