using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.CommissionRules.UpdateCommissionRule;

public record UpdateCommissionRuleCommand(
    Guid Id,
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
    DateTime? ValidTo) : IRequest<Result>;
