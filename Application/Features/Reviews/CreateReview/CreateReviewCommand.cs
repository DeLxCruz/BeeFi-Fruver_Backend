using Domain.Primitives;
using MediatR;

namespace Application.Features.Reviews.CreateReview;

public record CreateReviewCommand(
    Guid OrderId,
    Guid FruverId,
    int Rating,
    string? Comment) : IRequest<Result<Guid>>;
