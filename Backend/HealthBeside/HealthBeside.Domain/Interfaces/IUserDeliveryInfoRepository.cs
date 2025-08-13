using HealthBeside.Domain.Models.Users;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IUserDeliveryInfoRepository : IGenericRepository<UserDeliveryInfo>
{
    Task<UserDeliveryInfo?> GetByDetailsAsync(
        Guid userId,
        string city,
        string streetName,
        int streetNumber,
        CancellationToken cancellationToken = default);

    Task<UserDeliveryInfo?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}