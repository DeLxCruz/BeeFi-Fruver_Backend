using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cart.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCartQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var items = await _context.CartItems
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Include(c => c.FruverProduct)
                .ThenInclude(fp => fp.Product)
            .Include(c => c.FruverProduct)
                .ThenInclude(fp => fp.Fruver)
            .OrderBy(c => c.AddedAt)
            .ToListAsync(cancellationToken);

        var itemDtos = items.Select(c =>
        {
            var fp = c.FruverProduct;
            var finalPrice = fp.Price
                * (1 - (fp.DiscountPercentage ?? 0) / 100m)
                * (1 - (fp.BeeFiExclusiveDiscount ?? 0) / 100m);

            return new CartItemDto(
                c.Id,
                fp.Id,
                fp.Product.Name,
                fp.Product.MainImageUrl,
                fp.FruverId,
                $"{fp.Fruver.FirstName} {fp.Fruver.LastName}",
                fp.Price,
                fp.DiscountPercentage,
                fp.BeeFiExclusiveDiscount,
                Math.Round(finalPrice, 2),
                c.Quantity,
                Math.Round(finalPrice * c.Quantity, 2),
                fp.Stock,
                fp.IsAvailable,
                c.AddedAt);
        }).ToList();

        var fruverIds = itemDtos.Select(i => i.FruverId).Distinct().ToList();
        var hasMultipleFruvers = fruverIds.Count > 1;
        var subtotal = itemDtos.Sum(i => i.Subtotal);
        var updatedAt = items.Count > 0
            ? items.Max(c => c.UpdatedAt ?? c.AddedAt)
            : (DateTime?)null;

        var dto = new CartDto(
            userId,
            itemDtos,
            items.Count,
            Math.Round(subtotal, 2),
            hasMultipleFruvers,
            updatedAt);

        return Result.Success(dto);
    }
}
