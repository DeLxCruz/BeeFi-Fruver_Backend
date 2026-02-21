using Application.Common.Interfaces;
using FluentValidation;

namespace Application.Features.Categories.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la categoría es requerido")
            .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden de visualización no puede ser negativo");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null)
            .WithMessage("La descripción no puede superar 500 caracteres");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).When(x => x.ImageUrl is not null)
            .WithMessage("La URL de la imagen no puede superar 500 caracteres");
    }
}
