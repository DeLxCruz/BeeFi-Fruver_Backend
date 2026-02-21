using Domain.Primitives;
using MediatR;

namespace Application.Features.Reviews.DeleteReview;

public record DeleteReviewCommand(Guid ReviewId) : IRequest<Result>;
