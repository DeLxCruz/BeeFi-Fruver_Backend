using Application.Common.Interfaces;
using Domain.Constants;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deliveries.GetDeliveryByOrder;

public class GetDeliveryByOrderQueryHandler
    : IRequestHandler<GetDeliveryByOrderQuery, Result<DeliveryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDeliveryByOrderQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<DeliveryDto>> Handle(
        GetDeliveryByOrderQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        var isAdminOrEmpleado = _currentUser.Roles.Contains(Roles.Administrador)
            || _currentUser.Roles.Contains(Roles.Empleado);

        var delivery = await _context.Deliveries
            .AsNoTracking()
            .Include(d => d.Order)
            .Include(d => d.DeliveryPerson)
            .Include(d => d.StatusHistory.OrderByDescending(h => h.Timestamp))
            .FirstOrDefaultAsync(d => d.OrderId == request.OrderId, cancellationToken);

        if (delivery is null)
            return Result.Failure<DeliveryDto>(DeliveryErrors.NotFound);

        if (!isAdminOrEmpleado && delivery.Order.UserId != userId)
            return Result.Failure<DeliveryDto>(DeliveryErrors.NotFound);

        var historyDtos = delivery.StatusHistory
            .OrderByDescending(h => h.Timestamp)
            .Select(h => new DeliveryStatusHistoryDto(
                h.Status,
                h.Timestamp,
                h.Latitude,
                h.Longitude,
                h.Notes,
                h.UpdatedBy))
            .ToList();

        var deliveryPersonName = delivery.DeliveryPerson is not null
            ? $"{delivery.DeliveryPerson.FirstName} {delivery.DeliveryPerson.LastName}"
            : null;

        return Result.Success(new DeliveryDto(
            delivery.Id,
            delivery.OrderId,
            delivery.Order.OrderNumber,
            delivery.DeliveryPersonId,
            deliveryPersonName,
            delivery.Status,
            delivery.EstimatedDeliveryTime,
            delivery.ActualDeliveryTime,
            delivery.TrackingNotes,
            historyDtos));
    }
}
