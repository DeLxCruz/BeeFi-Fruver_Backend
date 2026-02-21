namespace Application.Features.Products.GetProducts;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    string CategoryName,
    string ImageUrl,
    string UnitOfMeasure,
    bool IsActive,
    DateTime CreatedAt,
    int TotalFruverCount);
