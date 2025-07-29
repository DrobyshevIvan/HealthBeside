using HealthBeside.Domain.Models.Users;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetRefreshTokenByUserId(Guid userId);
}