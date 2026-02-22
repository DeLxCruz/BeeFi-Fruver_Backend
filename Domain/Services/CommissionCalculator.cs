using Domain.Entities;
using Domain.Enums;

namespace Domain.Services;

public record CommissionResult(
    decimal CommissionAmount,
    string RuleApplied,
    Guid? RuleId);

public static class CommissionCalculator
{
    public static CommissionResult Calculate(
        List<CommissionRule> rules,
        Guid roleId,
        Guid zoneId,
        Guid categoryId,
        decimal orderAmount)
    {
        var applicable = rules
            .Where(r => r.IsApplicable(roleId, zoneId, categoryId, orderAmount))
            .OrderByDescending(r => r.Priority)
            .FirstOrDefault();

        if (applicable is null)
            return new CommissionResult(0m, "No rule applied", null);

        var amount = applicable.CommissionType switch
        {
            CommissionType.Percentage => orderAmount * applicable.CommissionValue / 100m,
            CommissionType.FixedAmount => applicable.CommissionValue,
            _ => 0m
        };

        if (applicable.MinCommission.HasValue && amount < applicable.MinCommission.Value)
            amount = applicable.MinCommission.Value;

        if (applicable.MaxCommission.HasValue && amount > applicable.MaxCommission.Value)
            amount = applicable.MaxCommission.Value;

        return new CommissionResult(
            Math.Round(amount, 4),
            applicable.Name,
            applicable.Id);
    }
}
