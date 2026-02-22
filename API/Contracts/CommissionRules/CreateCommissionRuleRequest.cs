using Domain.Enums;

namespace API.Contracts.CommissionRules;

public record CreateCommissionRuleRequest(
    string Name,
    Guid? RoleId,
    Guid? ZoneId,
    Guid? CategoryId,
    decimal? MinOrderAmount,
    decimal? MaxOrderAmount,
    CommissionType CommissionType,
    decimal CommissionValue,
    decimal? MinCommission,
    decimal? MaxCommission,
    int Priority,
    DateTime? ValidFrom,
    DateTime? ValidTo);
