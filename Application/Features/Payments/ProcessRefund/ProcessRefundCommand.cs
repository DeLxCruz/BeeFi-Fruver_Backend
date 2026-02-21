using Domain.Primitives;
using MediatR;

namespace Application.Features.Payments.ProcessRefund;

public record ProcessRefundCommand(
    Guid OrderId,
    decimal Amount,
    string Reason) : IRequest<Result>;
