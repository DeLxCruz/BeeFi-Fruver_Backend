using Domain.Primitives;
using MediatR;

namespace Application.Features.Loyalty.EarnPoints;

public record EarnPointsCommand(
    Guid UserId,
    Guid OrderId,
    decimal OrderTotal) : IRequest<Result>;
