using HealthBeside.Application.Contracts.User;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Interfaces;

public interface IUserDeliveryInfoService
{
    public Task<GetUserDeliveryInfoDto?> GetUserDeliveryInfoAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<GetUserDeliveryInfoDto?> GetUserDeliveryInfoByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<GetUserDeliveryInfoDto> CreateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default);
    public Task<GetUserDeliveryInfoDto> UpdateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default);
    public Task<GetUserDeliveryInfoDto> DeleteUserDeliveryInfoAsync(Guid id, CancellationToken cancellationToken = default);
}