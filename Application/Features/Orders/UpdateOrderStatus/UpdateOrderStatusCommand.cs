using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Orders.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus,
    string? Notes) : IRequest<Result>;
