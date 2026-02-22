using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ReturnRequests.ReviewReturnRequest;

public class ReviewReturnRequestCommandHandler
    : IRequestHandler<ReviewReturnRequestCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ReviewReturnRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        ReviewReturnRequestCommand request,
        CancellationToken cancellationToken)
    {
        var adminId = _currentUser.UserId!.Value;

        var returnRequest = await _context.ReturnRequests
            .FirstOrDefaultAsync(rr => rr.Id == request.ReturnRequestId, cancellationToken);

        if (returnRequest is null)
            return Result.Failure(ReturnRequestErrors.NotFound);

        if (returnRequest.Status != ReturnStatus.Pending)
            return Result.Failure(ReturnRequestErrors.AlreadyReviewed);

        if (request.Approve)
            returnRequest.Approve(adminId, request.RefundType, request.RefundAmount, request.Notes);
        else
            returnRequest.Reject(adminId, request.Notes);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
