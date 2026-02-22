using Domain.Enums;

namespace Application.Features.CommissionRules.GetCommissionRules;

public record CommissionRuleDto(
    Guid Id,
    string Name,
    Guid? RoleId,
    Guid? ZoneId,
    string? ZoneName,
    Guid? CategoryId,
    string? CategoryName,
    decimal? MinOrderAmount,
    decimal? MaxOrderAmount,
    CommissionType CommissionType,
    decimal CommissionValue,
    decimal? MinCommission,
    decimal? MaxCommission,
    int Priority,
    bool IsActive,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    DateTime CreatedAt);
