using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reviews.GetFruverReviews;

public class GetFruverReviewsQueryHandler
    : IRequestHandler<GetFruverReviewsQuery, Result<PaginatedList<ReviewDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetFruverReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ReviewDto>>> Handle(
        GetFruverReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Reviews
            .AsNoTracking()
            .Where(r => r.FruverId == request.FruverId && r.IsVisible)
            .Include(r => r.User);

        var totalCount = await query.CountAsync(cancellationToken);

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = reviews.Select(r => new ReviewDto(
            r.Id, r.UserId,
            $"{r.User.FirstName} {r.User.LastName}",
            r.FruverId, r.OrderId,
            r.Rating, r.Comment, r.CreatedAt))
            .ToList();

        return Result.Success(new PaginatedList<ReviewDto>(dtos, totalCount, request.PageNumber, request.PageSize));
    }
}
