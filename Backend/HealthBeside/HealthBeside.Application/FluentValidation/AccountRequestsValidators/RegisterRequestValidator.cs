using FluentValidation;
using HealthBeside.Application.Contracts;

namespace HealthBeside.Application.FluentValidation.AccountRequestsValidators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestBase>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.LastName).NotEmpty().MinimumLength(2);
    }
}