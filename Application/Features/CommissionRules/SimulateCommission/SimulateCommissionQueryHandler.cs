using Application.Common.Interfaces;
using Domain.Primitives;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CommissionRules.SimulateCommission;

public class SimulateCommissionQueryHandler
    : IRequestHandler<SimulateCommissionQuery, Result<SimulateCommissionResult>>
{
    private readonly IApplicationDbContext _context;

    public SimulateCommissionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SimulateCommissionResult>> Handle(
        SimulateCommissionQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var rules = await _context.CommissionRules
            .Where(cr => cr.IsActive &&
                         (cr.ValidFrom == null || cr.ValidFrom <= now) &&
                         (cr.ValidTo == null || cr.ValidTo >= now))
            .ToListAsync(cancellationToken);

        var result = CommissionCalculator.Calculate(
            rules,
            request.RoleId,
            request.ZoneId,
            request.CategoryId,
            request.OrderAmount);

        return Result.Success(new SimulateCommissionResult(
            result.CommissionAmount,
            result.RuleApplied,
            result.RuleId,
            result.RuleId.HasValue));
    }
}
