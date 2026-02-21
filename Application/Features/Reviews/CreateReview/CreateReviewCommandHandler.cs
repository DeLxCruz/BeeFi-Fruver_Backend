using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reviews.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateReviewCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null || order.UserId != userId)
            return Result.Failure<Guid>(ReviewErrors.NotOwner);

        if (order.Status != OrderStatus.Delivered)
            return Result.Failure<Guid>(ReviewErrors.OrderNotDelivered);

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.UserId == userId && r.OrderId == request.OrderId, cancellationToken);

        if (alreadyReviewed)
            return Result.Failure<Guid>(ReviewErrors.AlreadyReviewed);

        // Verify FruverId has at least one item in the order
        var fruverInOrder = order.Items.Any(i => i.FruverId == request.FruverId);
        if (!fruverInOrder)
            return Result.Failure<Guid>(new Error("Review.InvalidFruver", "El fruver no participa en este pedido"));

        var review = Review.Create(userId, request.FruverId, request.OrderId, request.Rating, request.Comment);
        _context.Reviews.Add(review);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(review.Id);
    }
}
