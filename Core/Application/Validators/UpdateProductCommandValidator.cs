using FluentValidation;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

namespace botilleria_clean_architecture_api.Core.Application.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID del producto debe ser mayor a 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

        RuleFor(x => x.DiscountPrice)
            .GreaterThanOrEqualTo(0).When(x => x.DiscountPrice.HasValue)
            .WithMessage("El precio de descuento debe ser mayor o igual a 0.")
            .LessThanOrEqualTo(x => x.Price).When(x => x.DiscountPrice.HasValue)
            .WithMessage("El precio de descuento no puede ser mayor al precio original.");

        RuleFor(x => x.Volume)
            .GreaterThan(0).When(x => x.Volume.HasValue)
            .WithMessage("El volumen debe ser mayor a 0.");

        RuleFor(x => x.Unit)
            .MaximumLength(10).WithMessage("La unidad no puede exceder 10 caracteres.");

        RuleFor(x => x.AlcoholContent)
            .InclusiveBetween(0, 100).When(x => x.AlcoholContent.HasValue)
            .WithMessage("El contenido de alcohol debe estar entre 0 y 100.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock debe ser mayor o igual a 0.");

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("El color no puede exceder 50 caracteres.");

        RuleFor(x => x.Aroma)
            .MaximumLength(500).WithMessage("El aroma no puede exceder 500 caracteres.");

        RuleFor(x => x.Taste)
            .MaximumLength(500).WithMessage("El sabor no puede exceder 500 caracteres.");

        RuleFor(x => x.ServingTemperature)
            .MaximumLength(50).WithMessage("La temperatura de servicio no puede exceder 50 caracteres.");
    }
}