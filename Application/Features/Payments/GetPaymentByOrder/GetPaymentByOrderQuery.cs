using Domain.Primitives;
using MediatR;

namespace Application.Features.Payments.GetPaymentByOrder;

public record GetPaymentByOrderQuery(Guid OrderId) : IRequest<Result<PaymentDto>>;
