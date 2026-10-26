using EnterpriseServiceManagement.DTOs;
using FluentValidation;



namespace EnterpriseServiceManagement.Validators;

public class CreateEmployeeValidator :AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 Characters.");

        RuleFor(x => x.LastName)
    .NotEmpty()
    .WithMessage("Last name is required.")
    .MaximumLength(50)
    .WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Enter a valid email address.")
            .MaximumLength(150);

        RuleFor(x => x.EmployeeCode)
            .NotEmpty()
            .WithMessage("Employee code is required.")
            .MinimumLength(2)
            .MaximumLength(20);

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("Department is required.");
    }
}





