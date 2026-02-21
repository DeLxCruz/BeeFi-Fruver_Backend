using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.UpdateStock;

public class UpdateStockCommandHandler
    : IRequestHandler<UpdateStockCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateStockCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateStockCommand request,
        CancellationToken cancellationToken)
    {
        var fruverId = _currentUser.UserId!.Value;

        var fruverProduct = await _context.FruverProducts
            .FirstOrDefaultAsync(fp => fp.Id == request.FruverProductId, cancellationToken);

        if (fruverProduct is null)
            return Result.Failure(FruverProductErrors.NotFound);

        if (fruverProduct.FruverId != fruverId)
            return Result.Failure(FruverProductErrors.NotOwner);

        fruverProduct.UpdateStock(request.NewStock);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
