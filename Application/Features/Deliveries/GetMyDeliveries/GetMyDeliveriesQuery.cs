using Application.Common.Models;
using Application.Features.Deliveries.GetDeliveryByOrder;
using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Deliveries.GetMyDeliveries;

public record GetMyDeliveriesQuery(
    DeliveryStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<DeliveryDto>>>;
