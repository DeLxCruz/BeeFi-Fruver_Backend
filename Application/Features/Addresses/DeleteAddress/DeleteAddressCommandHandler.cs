using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Addresses.DeleteAddress;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteAddressCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && !a.IsDeleted, cancellationToken);

        if (address is null)
            return Result.Failure(new Error("Address.NotFound", "La dirección no fue encontrada"));

        if (address.UserId != userId)
            return Result.Failure(new Error("Address.NotOwner", "No tienes permiso para eliminar esta dirección"));

        // Prevent deletion if there are active orders using this address
        var hasActiveOrders = await _context.Orders
            .AnyAsync(o =>
                o.AddressId == request.AddressId &&
                o.Status != OrderStatus.Delivered &&
                o.Status != OrderStatus.Cancelled,
                cancellationToken);

        if (hasActiveOrders)
            return Result.Failure(new Error("Address.HasActiveOrders",
                "No puedes eliminar una dirección asociada a pedidos activos"));

        address.Delete();
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
