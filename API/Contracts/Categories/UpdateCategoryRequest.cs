namespace API.Contracts.Categories;

public record UpdateCategoryRequest(
    string Name,
    string? Description,
    string? ImageUrl,
    bool IsActive,
    int DisplayOrder);
