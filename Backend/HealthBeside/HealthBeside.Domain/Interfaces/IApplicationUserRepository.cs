using HealthBeside.Domain.Models.Users;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IApplicationUserRepository : IGenericRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken);
}