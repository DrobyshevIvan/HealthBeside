using HealthBeside.Domain.Models.Shared;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetRefreshTokenByUserId(Guid userId);
}