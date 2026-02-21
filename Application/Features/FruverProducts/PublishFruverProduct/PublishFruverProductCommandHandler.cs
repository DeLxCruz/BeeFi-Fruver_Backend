using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.FruverProducts.PublishFruverProduct;

public class PublishFruverProductCommandHandler
    : IRequestHandler<PublishFruverProductCommand, Result<PublishFruverProductResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public PublishFruverProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PublishFruverProductResponse>> Handle(
        PublishFruverProductCommand request,
        CancellationToken cancellationToken)
    {
        var fruverId = _currentUser.UserId!.Value;

        // Verificar que el fruver está activo
        var fruver = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == fruverId, cancellationToken);

        if (fruver is null || fruver.AccountStatus != AccountStatus.Approved || !fruver.IsActive)
            return Result.Failure<PublishFruverProductResponse>(FruverProductErrors.FruverNotActive);

        // Verificar que el producto base existe y está activo
        var productExists = await _context.Products
            .AnyAsync(p => p.Id == request.ProductId && p.IsActive, cancellationToken);

        if (!productExists)
            return Result.Failure<PublishFruverProductResponse>(ProductErrors.NotFound);

        // Verificar que el fruver no tiene ya ese producto publicado
        var alreadyPublished = await _context.FruverProducts
            .AnyAsync(
                fp => fp.FruverId == fruverId && fp.ProductId == request.ProductId,
                cancellationToken);

        if (alreadyPublished)
            return Result.Failure<PublishFruverProductResponse>(FruverProductErrors.AlreadyExists);

        var fruverProduct = FruverProduct.Create(
            fruverId,
            request.ProductId,
            request.Price,
            request.Stock);

        if (request.DiscountPercentage > 0)
            fruverProduct.SetDiscount(request.DiscountPercentage);

        if (request.BeeFiExclusiveDiscount > 0)
            fruverProduct.SetBeeFiDiscount(request.BeeFiExclusiveDiscount);

        _context.FruverProducts.Add(fruverProduct);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new PublishFruverProductResponse(
            fruverProduct.Id,
            fruverProduct.ProductId,
            fruverProduct.Price,
            fruverProduct.Stock));
    }
}
