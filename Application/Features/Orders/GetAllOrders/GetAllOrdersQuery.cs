using Application.Common.Models;
using Application.Features.Orders.Common;
using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Orders.GetAllOrders;

public record GetAllOrdersQuery(
    int PageNumber = 1,
    int PageSize = 20,
    OrderStatus? Status = null,
    string? SearchTerm = null) : IRequest<Result<PaginatedList<OrderSummaryDto>>>;
