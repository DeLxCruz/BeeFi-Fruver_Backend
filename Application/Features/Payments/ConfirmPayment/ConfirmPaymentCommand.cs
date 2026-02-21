using Domain.Primitives;
using MediatR;

namespace Application.Features.Payments.ConfirmPayment;

public record ConfirmPaymentCommand(
    Guid OrderId,
    string TransactionId,
    string? GatewayResponse) : IRequest<Result>;
