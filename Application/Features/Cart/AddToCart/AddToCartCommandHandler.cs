using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cart.AddToCart;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddToCartCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var fruverProduct = await _context.FruverProducts
            .FirstOrDefaultAsync(fp => fp.Id == request.FruverProductId, cancellationToken);

        if (fruverProduct is null)
            return Result.Failure(CartErrors.ProductUnavailable);

        if (!fruverProduct.IsAvailable)
            return Result.Failure(CartErrors.ProductUnavailable);

        if (fruverProduct.Stock < request.Quantity)
            return Result.Failure(CartErrors.InsufficientStock);

        var existing = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.FruverProductId == request.FruverProductId,
                cancellationToken);

        if (existing is not null)
        {
            var newQuantity = existing.Quantity + request.Quantity;
            if (fruverProduct.Stock < newQuantity)
                return Result.Failure(CartErrors.InsufficientStock);

            existing.UpdateQuantity(newQuantity);
        }
        else
        {
            var cartItem = CartItem.Create(userId, request.FruverProductId, request.Quantity);
            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
