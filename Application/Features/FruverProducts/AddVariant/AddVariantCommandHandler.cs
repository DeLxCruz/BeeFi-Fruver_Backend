using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.AddVariant;

public class AddVariantCommandHandler : IRequestHandler<AddVariantCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddVariantCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        AddVariantCommand request,
        CancellationToken cancellationToken)
    {
        var fruverProduct = await _context.FruverProducts
            .FirstOrDefaultAsync(fp => fp.Id == request.FruverProductId, cancellationToken);

        if (fruverProduct is null)
            return Result.Failure<Guid>(FruverProductErrors.NotFound);

        if (fruverProduct.FruverId != _currentUser.UserId)
            return Result.Failure<Guid>(FruverProductErrors.NotOwner);

        var variant = ProductVariant.Create(
            request.FruverProductId,
            request.Name,
            request.PriceAdjustment,
            request.Stock,
            request.DisplayOrder,
            request.SKU);

        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(variant.Id);
    }
}
