namespace API.Contracts.Products;

public record CreateProductRequest(
    string Name,
    string? Description,
    Guid CategoryId,
    string? ImageUrl,
    string UnitOfMeasure);
