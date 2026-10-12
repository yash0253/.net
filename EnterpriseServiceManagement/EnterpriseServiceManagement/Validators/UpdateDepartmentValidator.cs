using EnterpriseServiceManagement.DTOs;
using FluentValidation;

namespace EnterpriseServiceManagement.Validators;

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MinimumLength(2)
            .WithMessage("Department name must contain at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("Department name cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Department code is required.")
            .MinimumLength(2)
            .WithMessage("Department code must contain at least 2 characters.")
            .MaximumLength(10)
            .WithMessage("Department code cannot exceed 10 characters.");
    }
}