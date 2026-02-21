using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reviews.DeleteReview;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteReviewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        review.Hide();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
