using Domain.Primitives;
using MediatR;

namespace Application.Features.CommissionRules.DeleteCommissionRule;

public record DeleteCommissionRuleCommand(Guid Id) : IRequest<Result>;
