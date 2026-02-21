namespace API.Contracts.Products;

public record UpdateProductRequest(
    string Name,
    string? Description,
    Guid CategoryId,
    string? ImageUrl,
    string UnitOfMeasure,
    bool IsActive);
