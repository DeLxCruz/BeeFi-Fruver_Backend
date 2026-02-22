using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.CommissionRules.CreateCommissionRule;

public record CreateCommissionRuleCommand(
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
    DateTime? ValidTo) : IRequest<Result<Guid>>;
