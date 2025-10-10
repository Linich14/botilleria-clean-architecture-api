using FluentValidation;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

namespace botilleria_clean_architecture_api.Core.Application.Validators;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la marca es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");
    }
}