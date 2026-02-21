using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
    Guid AddressId,
    PaymentMethod PaymentMethod,
    string? Notes) : IRequest<Result<Guid>>;
