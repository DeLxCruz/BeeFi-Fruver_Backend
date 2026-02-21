using Application.Common.Models;
using Domain.Primitives;
using MediatR;

namespace Application.Features.Reviews.GetFruverReviews;

public record GetFruverReviewsQuery(
    Guid FruverId,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<ReviewDto>>>;
