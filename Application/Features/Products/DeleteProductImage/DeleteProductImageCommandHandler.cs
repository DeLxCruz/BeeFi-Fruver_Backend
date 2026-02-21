using Application.Common.Interfaces;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.DeleteProductImage;

public class DeleteProductImageCommandHandler
    : IRequestHandler<DeleteProductImageCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductImageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var image = await _context.ProductImages
            .FirstOrDefaultAsync(i => i.Id == request.ImageId, cancellationToken);

        if (image is null)
            return Result.Failure(new Domain.Primitives.Error(
                "ProductImage.NotFound",
                "La imagen no fue encontrada"));

        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
