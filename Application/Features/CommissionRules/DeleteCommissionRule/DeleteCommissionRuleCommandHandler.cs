using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CommissionRules.DeleteCommissionRule;

public class DeleteCommissionRuleCommandHandler
    : IRequestHandler<DeleteCommissionRuleCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteCommissionRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteCommissionRuleCommand request,
        CancellationToken cancellationToken)
    {
        var rule = await _context.CommissionRules
            .FirstOrDefaultAsync(cr => cr.Id == request.Id, cancellationToken);

        if (rule is null)
            return Result.Failure(CommissionRuleErrors.NotFound);

        rule.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
