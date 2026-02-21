using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.AddProductImage;

public class AddProductImageCommandHandler
    : IRequestHandler<AddProductImageCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AddProductImageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        AddProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var productExists = await _context.Products
            .AnyAsync(p => p.Id == request.ProductId && p.IsActive, cancellationToken);

        if (!productExists)
            return Result.Failure(ProductErrors.NotFound);

        var image = ProductImage.Create(
            request.ProductId,
            request.ImageUrl,
            request.DisplayOrder);

        _context.ProductImages.Add(image);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
