using FluentValidation;

namespace Application.Features.Products.AddProductImage;

public class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("El Id del producto es requerido");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("La URL de la imagen es requerida")
            .MaximumLength(500).WithMessage("La URL no puede superar 500 caracteres");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden de visualización no puede ser negativo");
    }
}
