using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Payments.InitiatePayment;

public record InitiatePaymentCommand(
    Guid OrderId,
    PaymentMethod PaymentMethod,
    string ReturnUrl) : IRequest<Result<InitiatePaymentResponse>>;
