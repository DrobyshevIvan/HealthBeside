using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Interfaces;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken);
}