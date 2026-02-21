using FluentValidation;

namespace Application.Features.Products.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("La categoría es requerida");

        RuleFor(x => x.UnitOfMeasure)
            .NotEmpty().WithMessage("La unidad de medida es requerida")
            .MaximumLength(50).WithMessage("La unidad de medida no puede superar 50 caracteres");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).When(x => x.ImageUrl is not null)
            .WithMessage("La URL de la imagen no puede superar 500 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000).When(x => x.Description is not null)
            .WithMessage("La descripción no puede superar 1000 caracteres");
    }
}
