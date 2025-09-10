using FluentValidation;
using HealthBeside.Application.Contracts.User;

namespace HealthBeside.Application.FluentValidation.AccountRequestsValidators;

public class RegisterPatientRequestValidator : AbstractValidator<RegisterPatientProfileRequest>
{
    public RegisterPatientRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.LastName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.DateOfBirth).LessThan(DateTime.Now).WithMessage("Date of Birth must be in the past.");
        RuleFor(x => x.MedicalHistorySummary).MaximumLength(1000).WithMessage("Medical History Summary cannot exceed 1000 characters.");
    }
}