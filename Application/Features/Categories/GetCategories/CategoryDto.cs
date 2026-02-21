namespace Application.Features.Categories.GetCategories;

public record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    bool IsActive,
    int DisplayOrder,
    Guid? ParentCategoryId,
    List<CategoryDto> SubCategories
);
