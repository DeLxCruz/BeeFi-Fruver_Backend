using Domain.Primitives;
using MediatR;

namespace Application.Features.CommissionRules.GetCommissionRules;

public record GetCommissionRulesQuery(bool? IsActive = null)
    : IRequest<Result<List<CommissionRuleDto>>>;
