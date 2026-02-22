using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ReturnRequests.CreateReturnRequest;

public class CreateReturnRequestCommandHandler
    : IRequestHandler<CreateReturnRequestCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateReturnRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateReturnRequestCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.Failure<Guid>(OrderErrors.NotFound);

        if (order.UserId != userId)
            return Result.Failure<Guid>(ReturnRequestErrors.NotOwner);

        if (order.Status != OrderStatus.Delivered)
            return Result.Failure<Guid>(ReturnRequestErrors.OrderNotDelivered);

        var existingReturn = await _context.ReturnRequests
            .AnyAsync(rr => rr.OrderId == request.OrderId, cancellationToken);

        if (existingReturn)
            return Result.Failure<Guid>(ReturnRequestErrors.AlreadyExists);

        var returnRequest = ReturnRequest.Create(
            request.OrderId,
            userId,
            request.Reason,
            request.EvidenceUrl);

        _context.ReturnRequests.Add(returnRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(returnRequest.Id);
    }
}
