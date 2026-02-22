namespace API.Contracts.ReturnRequests;

public record CreateReturnRequestRequest(
    Guid OrderId,
    string Reason,
    string? EvidenceUrl = null);
