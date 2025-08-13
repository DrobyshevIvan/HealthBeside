using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Services;

public class UserDeliveryInfoService : IUserDeliveryInfoService
{
    private readonly IUserDeliveryInfoRepository _userDeliveryInfoService;

    public UserDeliveryInfoService(IUserDeliveryInfoRepository userDeliveryInfoService)
    {
        _userDeliveryInfoService = userDeliveryInfoService;
    }
    
    public Task<UserDeliveryInfo?> GetUserDeliveryInfoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userDeliveryInfo = _userDeliveryInfoService.GetByUserIdAsync(userId, cancellationToken);
        
        if(userDeliveryInfo is null)
            throw new KeyNotFoundException("User delivery info not found for the specified user ID.");
        
        return userDeliveryInfo;
    }

    public Task<UserDeliveryInfo?> GetUserDeliveryInfoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserDeliveryInfo> CreateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserDeliveryInfo> UpdateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserDeliveryInfoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}