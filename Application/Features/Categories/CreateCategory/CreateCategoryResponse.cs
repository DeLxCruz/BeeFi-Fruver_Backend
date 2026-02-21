namespace Application.Features.Categories.CreateCategory;

public record CreateCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    int DisplayOrder,
    Guid? ParentCategoryId
);
