using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.UpdateVariant;

public class UpdateVariantCommandHandler : IRequestHandler<UpdateVariantCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateVariantCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateVariantCommand request,
        CancellationToken cancellationToken)
    {
        var variant = await _context.ProductVariants
            .Include(pv => pv.FruverProduct)
            .FirstOrDefaultAsync(pv => pv.Id == request.VariantId, cancellationToken);

        if (variant is null)
            return Result.Failure(ProductVariantErrors.NotFound);

        if (variant.FruverProduct.FruverId != _currentUser.UserId)
            return Result.Failure(ProductVariantErrors.NotOwner);

        variant.Update(
            request.Name,
            request.PriceAdjustment,
            request.Stock,
            request.IsActive,
            request.DisplayOrder,
            request.SKU);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
