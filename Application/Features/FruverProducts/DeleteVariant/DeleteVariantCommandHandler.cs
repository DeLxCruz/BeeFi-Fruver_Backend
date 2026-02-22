using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.DeleteVariant;

public class DeleteVariantCommandHandler : IRequestHandler<DeleteVariantCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteVariantCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteVariantCommand request,
        CancellationToken cancellationToken)
    {
        var variant = await _context.ProductVariants
            .Include(pv => pv.FruverProduct)
            .FirstOrDefaultAsync(pv => pv.Id == request.VariantId, cancellationToken);

        if (variant is null)
            return Result.Failure(ProductVariantErrors.NotFound);

        if (variant.FruverProduct.FruverId != _currentUser.UserId)
            return Result.Failure(ProductVariantErrors.NotOwner);

        variant.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
