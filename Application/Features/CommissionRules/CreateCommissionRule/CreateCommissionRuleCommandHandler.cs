using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Primitives;
using MediatR;

namespace Application.Features.CommissionRules.CreateCommissionRule;

public class CreateCommissionRuleCommandHandler
    : IRequestHandler<CreateCommissionRuleCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateCommissionRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        CreateCommissionRuleCommand request,
        CancellationToken cancellationToken)
    {
        var rule = CommissionRule.Create(
            request.Name,
            request.CommissionType,
            request.CommissionValue,
            request.Priority);

        rule.Update(
            request.Name,
            request.RoleId,
            request.ZoneId,
            request.CategoryId,
            request.MinOrderAmount,
            request.MaxOrderAmount,
            request.CommissionType,
            request.CommissionValue,
            request.MinCommission,
            request.MaxCommission,
            request.Priority,
            true,
            request.ValidFrom,
            request.ValidTo);

        _context.CommissionRules.Add(rule);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(rule.Id);
    }
}
