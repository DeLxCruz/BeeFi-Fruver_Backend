using Domain.Primitives;
using MediatR;

namespace Application.Features.Orders.CancelOrder;

public record CancelOrderCommand(
    Guid OrderId,
    string? Reason) : IRequest<Result>;
