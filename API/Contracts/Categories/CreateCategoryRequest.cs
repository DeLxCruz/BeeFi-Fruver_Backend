namespace API.Contracts.Categories;

public record CreateCategoryRequest(
    string Name,
    string? Description,
    string? ImageUrl,
    Guid? ParentCategoryId,
    int DisplayOrder);
