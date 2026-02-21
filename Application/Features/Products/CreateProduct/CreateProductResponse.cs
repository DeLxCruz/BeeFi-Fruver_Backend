namespace Application.Features.Products.CreateProduct;

public record CreateProductResponse(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    string ImageUrl,
    string UnitOfMeasure);
