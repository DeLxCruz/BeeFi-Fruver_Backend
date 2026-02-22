using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.ReturnRequests.ReviewReturnRequest;

public record ReviewReturnRequestCommand(
    Guid ReturnRequestId,
    bool Approve,
    string? Notes,
    RefundType RefundType = RefundType.FullRefund,
    decimal? RefundAmount = null) : IRequest<Result>;
