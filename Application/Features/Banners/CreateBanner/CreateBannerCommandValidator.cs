using FluentValidation;

namespace Application.Features.Banners.CreateBanner;

public class CreateBannerCommandValidator : AbstractValidator<CreateBannerCommand>
{
    public CreateBannerCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es requerido.")
            .MaximumLength(200).WithMessage("El título no puede superar 200 caracteres.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("La URL de la imagen es requerida.")
            .MaximumLength(500).WithMessage("La URL no puede superar 500 caracteres.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden de visualización debe ser 0 o mayor.");
    }
}
