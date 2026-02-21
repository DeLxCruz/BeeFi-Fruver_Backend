using Application.Common.Interfaces;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.UnpublishFruverProduct;

public class UnpublishFruverProductCommandHandler
    : IRequestHandler<UnpublishFruverProductCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UnpublishFruverProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UnpublishFruverProductCommand request,
        CancellationToken cancellationToken)
    {
        var fruverId = _currentUser.UserId!.Value;

        var fruverProduct = await _context.FruverProducts
            .FirstOrDefaultAsync(fp => fp.Id == request.FruverProductId, cancellationToken);

        if (fruverProduct is null)
            return Result.Failure(FruverProductErrors.NotFound);

        if (fruverProduct.FruverId != fruverId)
            return Result.Failure(FruverProductErrors.NotOwner);

        fruverProduct.MakeUnavailable();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
