using Domain.Enums;

namespace API.Contracts.CommissionRules;

public record UpdateCommissionRuleRequest(
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
    bool IsActive,
    DateTime? ValidFrom,
    DateTime? ValidTo);
