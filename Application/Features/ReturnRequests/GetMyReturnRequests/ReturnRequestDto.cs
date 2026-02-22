using Domain.Enums;

namespace Application.Features.ReturnRequests.GetMyReturnRequests;

public record ReturnRequestDto(
    Guid Id,
    Guid OrderId,
    string OrderNumber,
    string Reason,
    string? EvidenceUrl,
    ReturnStatus Status,
    string? AdminNotes,
    RefundType RefundType,
    decimal? RefundAmount,
    DateTime CreatedAt,
    DateTime? ReviewedAt);
