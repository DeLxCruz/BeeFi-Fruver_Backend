using Domain.Enums;

namespace API.Contracts.ReturnRequests;

public record ReviewReturnRequestRequest(
    bool Approve,
    string? Notes,
    RefundType RefundType = RefundType.FullRefund,
    decimal? RefundAmount = null);
