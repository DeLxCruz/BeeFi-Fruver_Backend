using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities;

public class CommissionRule : Entity
{
    public string Name { get; private set; } = null!;
    public Guid? RoleId { get; private set; }
    public Guid? ZoneId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public decimal? MinOrderAmount { get; private set; }
    public decimal? MaxOrderAmount { get; private set; }
    public CommissionType CommissionType { get; private set; }
    public decimal CommissionValue { get; private set; }
    public decimal? MinCommission { get; private set; }
    public decimal? MaxCommission { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }

    // Navigation properties
    public virtual Role? Role { get; set; }
    public virtual Zone? Zone { get; set; }
    public virtual Category? Category { get; set; }

    private CommissionRule() { }

    private CommissionRule(Guid id) : base(id) { }

    public static CommissionRule Create(
        string name,
        CommissionType commissionType,
        decimal commissionValue,
        int priority)
    {
        return new CommissionRule(Guid.NewGuid())
        {
            Name = name,
            CommissionType = commissionType,
            CommissionValue = commissionValue,
            Priority = priority,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        Guid? roleId,
        Guid? zoneId,
        Guid? categoryId,
        decimal? minOrderAmount,
        decimal? maxOrderAmount,
        CommissionType commissionType,
        decimal commissionValue,
        decimal? minCommission,
        decimal? maxCommission,
        int priority,
        bool isActive,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Name = name;
        RoleId = roleId;
        ZoneId = zoneId;
        CategoryId = categoryId;
        MinOrderAmount = minOrderAmount;
        MaxOrderAmount = maxOrderAmount;
        CommissionType = commissionType;
        CommissionValue = commissionValue;
        MinCommission = minCommission;
        MaxCommission = maxCommission;
        Priority = priority;
        IsActive = isActive;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public void Deactivate() => IsActive = false;

    public bool IsApplicable(Guid? roleId, Guid? zoneId, Guid? categoryId, decimal orderAmount)
    {
        if (!IsActive) return false;

        var now = DateTime.UtcNow;
        if (ValidFrom.HasValue && now < ValidFrom.Value) return false;
        if (ValidTo.HasValue && now > ValidTo.Value) return false;

        if (RoleId.HasValue && RoleId != roleId) return false;
        if (ZoneId.HasValue && ZoneId != zoneId) return false;
        if (CategoryId.HasValue && CategoryId != categoryId) return false;

        if (MinOrderAmount.HasValue && orderAmount < MinOrderAmount.Value) return false;
        if (MaxOrderAmount.HasValue && orderAmount > MaxOrderAmount.Value) return false;

        return true;
    }
}
