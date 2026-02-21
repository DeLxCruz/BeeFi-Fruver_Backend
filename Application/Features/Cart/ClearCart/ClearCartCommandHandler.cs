using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cart.ClearCart;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ClearCartCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);

        if (cartItems.Count > 0)
        {
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
