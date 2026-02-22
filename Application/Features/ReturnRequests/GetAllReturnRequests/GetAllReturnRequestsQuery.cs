using Domain.Enums;
using Domain.Primitives;
using MediatR;

namespace Application.Features.ReturnRequests.GetAllReturnRequests;

public record GetAllReturnRequestsQuery(ReturnStatus? Status = null)
    : IRequest<Result<List<AllReturnRequestDto>>>;

public record AllReturnRequestDto(
    Guid Id,
    Guid OrderId,
    string OrderNumber,
    Guid UserId,
    string UserName,
    string Reason,
    string? EvidenceUrl,
    ReturnStatus Status,
    string? AdminNotes,
    RefundType RefundType,
    decimal? RefundAmount,
    DateTime CreatedAt,
    DateTime? ReviewedAt);
