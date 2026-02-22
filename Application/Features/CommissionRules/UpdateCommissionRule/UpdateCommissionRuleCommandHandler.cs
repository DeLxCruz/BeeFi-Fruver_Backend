using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CommissionRules.UpdateCommissionRule;

public class UpdateCommissionRuleCommandHandler
    : IRequestHandler<UpdateCommissionRuleCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateCommissionRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateCommissionRuleCommand request,
        CancellationToken cancellationToken)
    {
        var rule = await _context.CommissionRules
            .FirstOrDefaultAsync(cr => cr.Id == request.Id, cancellationToken);

        if (rule is null)
            return Result.Failure(CommissionRuleErrors.NotFound);

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
            request.IsActive,
            request.ValidFrom,
            request.ValidTo);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
