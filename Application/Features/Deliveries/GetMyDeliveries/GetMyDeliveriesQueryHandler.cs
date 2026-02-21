using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Deliveries.GetDeliveryByOrder;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Deliveries.GetMyDeliveries;

public class GetMyDeliveriesQueryHandler
    : IRequestHandler<GetMyDeliveriesQuery, Result<PaginatedList<DeliveryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyDeliveriesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<DeliveryDto>>> Handle(
        GetMyDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var query = _context.Deliveries
            .AsNoTracking()
            .Where(d => d.DeliveryPersonId == userId)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(d => d.Status == request.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var deliveries = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(d => d.Order)
            .Include(d => d.DeliveryPerson)
            .Include(d => d.StatusHistory)
            .ToListAsync(cancellationToken);

        var dtos = deliveries.Select(d =>
        {
            var historyDtos = d.StatusHistory
                .OrderByDescending(h => h.Timestamp)
                .Select(h => new DeliveryStatusHistoryDto(
                    h.Status, h.Timestamp, h.Latitude, h.Longitude, h.Notes, h.UpdatedBy))
                .ToList();

            var personName = d.DeliveryPerson is not null
                ? $"{d.DeliveryPerson.FirstName} {d.DeliveryPerson.LastName}"
                : null;

            return new DeliveryDto(
                d.Id, d.OrderId, d.Order.OrderNumber,
                d.DeliveryPersonId, personName,
                d.Status, d.EstimatedDeliveryTime, d.ActualDeliveryTime,
                d.TrackingNotes, historyDtos);
        }).ToList();

        return Result.Success(new PaginatedList<DeliveryDto>(dtos, totalCount, request.PageNumber, request.PageSize));
    }
}
