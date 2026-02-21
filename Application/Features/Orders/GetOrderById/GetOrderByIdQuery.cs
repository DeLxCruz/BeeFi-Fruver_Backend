using Application.Features.Orders.Common;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Orders.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderDetailDto>>;
