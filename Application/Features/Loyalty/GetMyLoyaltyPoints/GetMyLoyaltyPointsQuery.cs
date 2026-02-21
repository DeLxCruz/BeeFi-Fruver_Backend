using Domain.Primitives;
using MediatR;

namespace Application.Features.Loyalty.GetMyLoyaltyPoints;

public record GetMyLoyaltyPointsQuery : IRequest<Result<LoyaltyPointsDto>>;
