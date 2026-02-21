using Application.Common.Models;
using Application.Features.Deliveries.GetDeliveryByOrder;
using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.GetAllDeliveries;

public record GetAllDeliveriesQuery(
    DeliveryStatus? Status = null,
    Guid? DeliveryPersonId = null,
    Guid? ZoneId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<DeliveryDto>>>;
