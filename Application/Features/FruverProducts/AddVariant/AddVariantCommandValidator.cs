using FluentValidation;

namespace Application.Features.FruverProducts.AddVariant;

public class AddVariantCommandValidator : AbstractValidator<AddVariantCommand>
{
    public AddVariantCommandValidator()
    {
        RuleFor(x => x.FruverProductId)
            .NotEmpty().WithMessage("El ID del producto es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la variante es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

        RuleFor(x => x.SKU)
            .MaximumLength(50).When(x => x.SKU is not null)
            .WithMessage("El SKU no puede superar 50 caracteres.");
    }
}
