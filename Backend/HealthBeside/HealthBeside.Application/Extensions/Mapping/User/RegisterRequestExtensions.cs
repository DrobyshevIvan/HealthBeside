using HealthBeside.Application.Contracts;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Extensions.Mapping.User;

public static class RegisterRequestExtensions
{
    public static (string? error, ApplicationUser user) ToApplicationUser(this RegisterRequest request)
    {
        return ApplicationUser.Create(
            request.FirstName, 
            request.LastName, 
            request.Email);
    }
}