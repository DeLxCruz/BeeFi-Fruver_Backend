using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        // Load cart items with all needed navigation
        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.FruverProduct)
                .ThenInclude(fp => fp.Product)
            .Include(c => c.FruverProduct)
                .ThenInclude(fp => fp.Fruver)
            .ToListAsync(cancellationToken);

        if (cartItems.Count == 0)
            return Result.Failure<Guid>(CartErrors.Empty);

        // Validate address ownership
        var address = await _context.Addresses
            .Include(a => a.Zone)
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && !a.IsDeleted,
                cancellationToken);

        if (address is null)
            return Result.Failure<Guid>(OrderErrors.AddressNotFound);

        if (address.UserId != userId)
            return Result.Failure<Guid>(OrderErrors.AddressNotOwner);

        // Validate stock and availability
        foreach (var item in cartItems)
        {
            var fp = item.FruverProduct;
            if (!fp.IsAvailable)
                return Result.Failure<Guid>(CartErrors.ProductUnavailable);
            if (fp.Stock < item.Quantity)
                return Result.Failure<Guid>(CartErrors.InsufficientStock);
        }

        // Calculate totals
        decimal subtotal = 0;
        foreach (var item in cartItems)
        {
            var fp = item.FruverProduct;
            var finalPrice = fp.Price
                * (1 - (fp.DiscountPercentage ?? 0) / 100m)
                * (1 - (fp.BeeFiExclusiveDiscount ?? 0) / 100m);
            subtotal += Math.Round(finalPrice, 2) * item.Quantity;
        }

        var deliveryFee = address.Zone.DeliveryBaseCost;
        var orderNumber = $"BF-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999):D4}";

        // Create order
        var order = Order.Create(
            orderNumber,
            userId,
            request.AddressId,
            Math.Round(subtotal, 2),
            deliveryFee,
            0m,
            0m,
            request.PaymentMethod,
            request.Notes);

        _context.Orders.Add(order);

        // Create order items and reduce stock
        foreach (var item in cartItems)
        {
            var fp = item.FruverProduct;
            var finalPrice = fp.Price
                * (1 - (fp.DiscountPercentage ?? 0) / 100m)
                * (1 - (fp.BeeFiExclusiveDiscount ?? 0) / 100m);

            var orderItem = OrderItem.Create(
                order.Id,
                fp.Id,
                fp.FruverId,
                item.Quantity,
                Math.Round(finalPrice, 2),
                fp.Product.Name,
                fp.Product.MainImageUrl);

            _context.OrderItems.Add(orderItem);
            fp.ReduceStock(item.Quantity);
        }

        // Clear cart
        _context.CartItems.RemoveRange(cartItems);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(order.Id);
    }
}
