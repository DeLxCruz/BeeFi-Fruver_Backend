namespace Application.Features.FruverProducts.GetCatalogByZone;

public record ZoneCatalogDto(
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
    Guid ZoneId,
    string ZoneName,
    decimal DeliveryBaseCost);
