using Domain.Primitives;
using MediatR;

namespace Application.Features.FruverProducts.GetFruverProductById;

public record FruverProductDetailDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductImageUrl,
    Guid CategoryId,
    string CategoryName,
    Guid FruverId,
    string FruverName,
    decimal Price,
    int Stock,
    bool IsAvailable,
    decimal DiscountPercentage,
    decimal BeeFiExclusiveDiscount,
    decimal FinalPrice,
    string UnitOfMeasure,
    int PreparationTimeMinutes,
    bool IsSeasonal,
    DateTime? AvailableFrom,
    DateTime? AvailableUntil,
    bool AllowPreOrder,
    DateTime? PreOrderAvailableDate,
    List<ProductVariantDto> Variants);

public record GetFruverProductByIdQuery(Guid FruverProductId) : IRequest<Result<FruverProductDetailDto>>;
