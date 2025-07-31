using HealthBeside.Domain.Models.Shared;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Interfaces;

public interface IApplicationUserRepository : IGenericRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken);
}