using HealthBeside.Application.Contracts;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Extensions.Mapping.User;

public static class RegisterRequestExtensions
{
    public static (string? error, ApplicationUser user) ToApplicationUser(this RegisterRequestBase requestBase)
    {
        return ApplicationUser.Create(
            requestBase.FirstName, 
            requestBase.LastName, 
            requestBase.Email);
    }
}