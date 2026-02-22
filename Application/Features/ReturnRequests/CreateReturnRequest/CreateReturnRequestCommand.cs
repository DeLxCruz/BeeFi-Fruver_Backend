using Domain.Primitives;
using MediatR;

namespace Application.Features.ReturnRequests.CreateReturnRequest;

public record CreateReturnRequestCommand(
    Guid OrderId,
    string Reason,
    string? EvidenceUrl = null) : IRequest<Result<Guid>>;
