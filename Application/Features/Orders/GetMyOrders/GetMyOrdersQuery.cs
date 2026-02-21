using Application.Common.Models;
using Domain.Primitives;
using MediatR;
using Application.Features.Orders.Common;

namespace Application.Features.Orders.GetMyOrders;

public record GetMyOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PaginatedList<OrderSummaryDto>>>;
