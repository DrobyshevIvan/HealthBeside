using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Interfaces;

public interface IUserDeliveryInfoService
{
    public Task<UserDeliveryInfo?> GetUserDeliveryInfoAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<UserDeliveryInfo?> GetUserDeliveryInfoByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<UserDeliveryInfo> CreateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default);
    public Task<UserDeliveryInfo> UpdateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default);
    public Task DeleteUserDeliveryInfoAsync(Guid id, CancellationToken cancellationToken = default);
}