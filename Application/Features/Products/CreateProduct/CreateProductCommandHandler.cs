using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateProductResponse>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que la categoría existe y está activa
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId && c.IsActive, cancellationToken);

        if (!categoryExists)
            return Result.Failure<CreateProductResponse>(CategoryErrors.NotFound);

        // Verificar nombre único dentro de la categoría
        var nameExists = await _context.Products
            .AnyAsync(
                p => p.Name == request.Name && p.CategoryId == request.CategoryId,
                cancellationToken);

        if (nameExists)
            return Result.Failure<CreateProductResponse>(ProductErrors.AlreadyExists);

        var product = Product.Create(
            request.Name,
            request.Description ?? string.Empty,
            request.CategoryId,
            request.ImageUrl ?? string.Empty,
            request.UnitOfMeasure);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.CategoryId,
            product.MainImageUrl,
            product.UnitOfMeasure));
    }
}
