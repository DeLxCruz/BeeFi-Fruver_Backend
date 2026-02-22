using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CommissionRules.GetCommissionRules;

public class GetCommissionRulesQueryHandler
    : IRequestHandler<GetCommissionRulesQuery, Result<List<CommissionRuleDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCommissionRulesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CommissionRuleDto>>> Handle(
        GetCommissionRulesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.CommissionRules.AsNoTracking();

        if (request.IsActive.HasValue)
            query = query.Where(cr => cr.IsActive == request.IsActive.Value);

        var rules = await query
            .OrderByDescending(cr => cr.Priority)
            .Select(cr => new CommissionRuleDto(
                cr.Id,
                cr.Name,
                cr.RoleId,
                cr.ZoneId,
                cr.Zone != null ? cr.Zone.Name : null,
                cr.CategoryId,
                cr.Category != null ? cr.Category.Name : null,
                cr.MinOrderAmount,
                cr.MaxOrderAmount,
                cr.CommissionType,
                cr.CommissionValue,
                cr.MinCommission,
                cr.MaxCommission,
                cr.Priority,
                cr.IsActive,
                cr.ValidFrom,
                cr.ValidTo,
                cr.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success(rules);
    }
}
