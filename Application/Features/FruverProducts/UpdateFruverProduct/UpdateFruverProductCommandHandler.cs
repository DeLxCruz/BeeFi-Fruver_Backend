using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.UpdateFruverProduct;

public class UpdateFruverProductCommandHandler
    : IRequestHandler<UpdateFruverProductCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateFruverProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateFruverProductCommand request,
        CancellationToken cancellationToken)
    {
        var fruverId = _currentUser.UserId!.Value;

        var fruverProduct = await _context.FruverProducts
            .FirstOrDefaultAsync(fp => fp.Id == request.FruverProductId, cancellationToken);

        if (fruverProduct is null)
            return Result.Failure(FruverProductErrors.NotFound);

        if (fruverProduct.FruverId != fruverId)
            return Result.Failure(FruverProductErrors.NotOwner);

        fruverProduct.Update(
            request.Price,
            request.Stock,
            request.DiscountPercentage > 0 ? request.DiscountPercentage : (decimal?)null,
            request.BeeFiExclusiveDiscount > 0 ? request.BeeFiExclusiveDiscount : (decimal?)null,
            request.IsAvailable);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
