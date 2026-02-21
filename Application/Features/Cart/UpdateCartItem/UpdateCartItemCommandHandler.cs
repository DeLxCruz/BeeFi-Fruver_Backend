using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cart.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateCartItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var cartItem = await _context.CartItems
            .Include(c => c.FruverProduct)
            .FirstOrDefaultAsync(c => c.Id == request.CartItemId && c.UserId == userId,
                cancellationToken);

        if (cartItem is null)
            return Result.Failure(CartErrors.NotFound);

        if (!cartItem.FruverProduct.IsAvailable)
            return Result.Failure(CartErrors.ProductUnavailable);

        if (cartItem.FruverProduct.Stock < request.Quantity)
            return Result.Failure(CartErrors.InsufficientStock);

        cartItem.UpdateQuantity(request.Quantity);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
