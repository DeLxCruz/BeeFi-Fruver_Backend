using Application.Common.Interfaces;
using Application.Features.Orders.Common;
using Domain.Constants;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetOrderByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<OrderDetailDto>> Handle(
        GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        var isAdminOrEmpleado = _currentUser.Roles.Contains(Roles.Administrador)
            || _currentUser.Roles.Contains(Roles.Empleado);

        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Address)
            .Include(o => o.Items)
                .ThenInclude(i => i.Fruver)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure<OrderDetailDto>(OrderErrors.NotFound);

        if (!isAdminOrEmpleado && order.UserId != userId)
            return Result.Failure<OrderDetailDto>(OrderErrors.NotOwner);

        var addressDetail = $"{order.Address.Label} - {order.Address.Street} {order.Address.HouseNumber}, {order.Address.AdditionalInfo}";

        var items = order.Items.Select(i => new OrderItemDto(
            i.Id,
            i.FruverProductId,
            i.ProductName,
            i.ProductImageUrl,
            i.FruverId,
            $"{i.Fruver.FirstName} {i.Fruver.LastName}",
            i.UnitPrice,
            i.Quantity,
            i.Subtotal)).ToList();

        var dto = new OrderDetailDto(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.Status.ToString(),
            order.AddressId,
            addressDetail,
            order.Subtotal,
            order.DeliveryFee,
            order.Discount,
            order.BeeFiDiscount,
            order.Total,
            order.PaymentMethod,
            order.PaymentStatus,
            order.Notes,
            order.CreatedAt,
            order.UpdatedAt,
            items);

        return Result.Success(dto);
    }
}
