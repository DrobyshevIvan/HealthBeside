using FluentValidation;
using HealthBeside.Application.Contracts;

namespace HealthBeside.Application.FluentValidation.AccountRequestsValidators;

public class AccountRequestValidator : AbstractValidator<LoginRequest>
{
    public AccountRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}