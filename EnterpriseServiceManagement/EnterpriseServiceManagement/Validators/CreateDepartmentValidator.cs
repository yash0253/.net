using EnterpriseServiceManagement.DTOs;
using FluentValidation;


namespace EnterpriseServiceManagement.Validators;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x =>x.Name)
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


//We don't want invalid requests reaching our service.

//For example, this should not be accepted:

//{
//  "name": "",
//  "code": ""
//}