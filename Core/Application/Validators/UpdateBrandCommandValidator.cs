using FluentValidation;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

namespace botilleria_clean_architecture_api.Core.Application.Validators;

public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID de la marca debe ser mayor a 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la marca es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");
    }
}