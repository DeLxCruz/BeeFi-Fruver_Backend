using Domain.Primitives;
using MediatR;

namespace Application.Features.CommissionRules.SimulateCommission;

public record SimulateCommissionQuery(
    Guid RoleId,
    Guid ZoneId,
    Guid CategoryId,
    decimal OrderAmount) : IRequest<Result<SimulateCommissionResult>>;

public record SimulateCommissionResult(
    decimal CommissionAmount,
    string RuleApplied,
    Guid? RuleId,
    bool HasRule);
