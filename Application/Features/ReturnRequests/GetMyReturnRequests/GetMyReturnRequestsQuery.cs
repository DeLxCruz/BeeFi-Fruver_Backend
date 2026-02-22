using Domain.Primitives;
using MediatR;

namespace Application.Features.ReturnRequests.GetMyReturnRequests;

public record GetMyReturnRequestsQuery : IRequest<Result<List<ReturnRequestDto>>>;
