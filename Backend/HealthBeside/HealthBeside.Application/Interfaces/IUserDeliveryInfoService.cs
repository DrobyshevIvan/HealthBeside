using HealthBeside.Application.Contracts.User;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Interfaces;

public interface IUserDeliveryInfoService
{
    public Task<UserDeliveryInfoDto?> GetUserDeliveryInfoAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<UserDeliveryInfoDto?> GetUserDeliveryInfoByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<UserDeliveryInfoDto> CreateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default);
    public Task<UserDeliveryInfoDto> UpdateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default);
    public Task<UserDeliveryInfoDto> DeleteUserDeliveryInfoAsync(Guid id, CancellationToken cancellationToken = default);
}