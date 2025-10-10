using FluentValidation;
using HealthBeside.Application.Contracts.User;

namespace HealthBeside.Application.FluentValidation.AccountRequestsValidators;

public class RegisterDoctorRequestValidator : AbstractValidator<RegisterDoctorProfileRequest>
{
    public RegisterDoctorRequestValidator()
    {
        // RuleFor(x => x.Email).NotEmpty().EmailAddress();
        // RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        // RuleFor(x => x.FirstName).NotEmpty().MinimumLength(2);
        // RuleFor(x => x.LastName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Specialization).NotEmpty().MinimumLength(2);
        RuleFor(x => x.MedicalLicenseNumber).NotEmpty().MinimumLength(5);
        RuleFor(x => x.ClinicAffiliation).NotEmpty().MinimumLength(2);
        RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0).WithMessage("Years of Experience must be a non-negative number.");
        RuleFor(x => x.Education).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Biography).MaximumLength(2000).WithMessage("Biography cannot exceed 2000 characters.");
    }
}