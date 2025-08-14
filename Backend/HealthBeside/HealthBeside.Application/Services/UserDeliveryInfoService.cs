using HealthBeside.Application.Contracts.User;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketOrderDto.Order;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class UserDeliveryInfoService : IUserDeliveryInfoService
{
    private readonly IUserDeliveryInfoRepository _userDeliveryInfoRepository;
    private readonly ILogger<UserDeliveryInfoService> _logger;

    public UserDeliveryInfoService(IUserDeliveryInfoRepository userDeliveryInfoRepository, ILogger<UserDeliveryInfoService> logger)
    {
        _userDeliveryInfoRepository = userDeliveryInfoRepository;
        _logger = logger;
    }
    
    //TODO: Consider adding a method to ensure that the user has ownership of the delivery info.
    private static void EnsureOwnership(UserDeliveryInfo deliveryInfo, Guid userId)
    {
        if (deliveryInfo.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }
    
    public async Task<UserDeliveryInfoDto?> GetUserDeliveryInfoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve user delivery info for user ID: {UserId}", userId);
        var userDeliveryInfo = await _userDeliveryInfoRepository.GetByUserIdAsync(userId, cancellationToken);
        
        if (userDeliveryInfo is null)
        {
            _logger.LogError("User delivery info not found for user ID: {UserId}", userId);
            throw new KeyNotFoundException("User delivery info not found for the specified user ID.");
        }
        
        _logger.LogInformation("Successfully retrieved user delivery info for user ID: {UserId}", userId);
        return userDeliveryInfo.ToGetUserDeliveryInfoDto();
    }

    public async Task<UserDeliveryInfoDto?> GetUserDeliveryInfoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve user delivery info by ID: {Id}", id);
        var userDeliveryInfo = await _userDeliveryInfoRepository.GetAsync(id, cancellationToken);

        if (userDeliveryInfo is null)
        {
            _logger.LogError("User delivery info not found for ID: {Id}", id);
            throw new UserDeliveryInfoException("User delivery info not found for the specified ID.");
        }

        _logger.LogInformation("Successfully retrieved user delivery info by ID: {Id}", id);
        return userDeliveryInfo.ToGetUserDeliveryInfoDto();
    }

    public async Task<UserDeliveryInfoDto> CreateUserDeliveryInfoAsync(UserDeliveryInfo userDeliveryInfo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create user delivery info for user ID: {UserId}", userDeliveryInfo.ApplicationUserId);
        
        var (errors, createdUserDeliveryInfo) = UserDeliveryInfo.Create(
            userDeliveryInfo.City,
            userDeliveryInfo.PhoneNumber,
            userDeliveryInfo.PostalIndex,
            userDeliveryInfo.StreetName,
            userDeliveryInfo.StreetNumber,
            userDeliveryInfo.ApplicationUserId);

        if (errors != null)
        {
            _logger.LogError("Failed to create user delivery info due to validation errors: {Errors}", errors);
            throw new UserDeliveryInfoException(errors);
        }

        if (createdUserDeliveryInfo is null)
        {
            _logger.LogError("Failed to create user delivery info due to unknown error for user ID: {UserId}", userDeliveryInfo.ApplicationUserId);
            throw new UserDeliveryInfoException("Failed to create user delivery info due to unknown error.");
        }
        
        var existingInfo = await _userDeliveryInfoRepository.GetByUserIdAsync(createdUserDeliveryInfo.ApplicationUserId, cancellationToken);
        if (existingInfo is not null)
        {
            _logger.LogError("User delivery info already exists for user ID: {UserId}", createdUserDeliveryInfo.ApplicationUserId);
            throw new UserDeliveryInfoException("User delivery info already exists for this user.");
        }
        
        var addedUserDeliveryInfo = await _userDeliveryInfoRepository.AddAsync(createdUserDeliveryInfo, cancellationToken);
        if (addedUserDeliveryInfo is null)
        {
            _logger.LogError("Failed to add user delivery info to the repository for user ID: {UserId}", createdUserDeliveryInfo.ApplicationUserId);
            throw new UserDeliveryInfoException("Failed to add user delivery info to the repository.");
        }

        _logger.LogInformation("Successfully created user delivery info for user ID: {UserId}", userDeliveryInfo.ApplicationUserId);
        return addedUserDeliveryInfo.ToGetUserDeliveryInfoDto();
    }

    public async Task<UserDeliveryInfoDto> UpdateUserDeliveryInfoAsync(
        UserDeliveryInfo userDeliveryInfo, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update user delivery info for user ID: {UserId}", userDeliveryInfo.ApplicationUserId);
        
        var existingInfo = await _userDeliveryInfoRepository.GetByUserIdAsync(
            userDeliveryInfo.ApplicationUserId, 
            cancellationToken);

        if (existingInfo is null)
        {
            _logger.LogError("User delivery info does not exist for user ID: {UserId}", userDeliveryInfo.ApplicationUserId);
            throw new UserDeliveryInfoException("User delivery info does not exist for this user.");
        }

        var updateError = existingInfo.Update(
            userDeliveryInfo.City,
            userDeliveryInfo.PhoneNumber,
            userDeliveryInfo.PostalIndex,
            userDeliveryInfo.StreetName,
            userDeliveryInfo.StreetNumber
        );

        if (updateError != null)
        {
            _logger.LogError("Failed to update user delivery info for user ID {UserId} due to validation errors: {Errors}", userDeliveryInfo.ApplicationUserId, updateError);
            throw new UserDeliveryInfoException(updateError);
        }

        await _userDeliveryInfoRepository.UpdateAsync(existingInfo, cancellationToken);

        _logger.LogInformation("Successfully updated user delivery info for user ID: {UserId}", userDeliveryInfo.ApplicationUserId);
        return existingInfo.ToGetUserDeliveryInfoDto();
    }

    public async Task<UserDeliveryInfoDto> DeleteUserDeliveryInfoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete user delivery info with ID: {Id}", id);
        
        var userDeliveryInfo = await _userDeliveryInfoRepository.GetAsync(id, cancellationToken);

        if (userDeliveryInfo is null)
        {
            _logger.LogError("User delivery info not found for deletion with ID: {Id}", id);
            throw new UserDeliveryInfoException("User delivery info not found for the specified ID.");
        }

        await _userDeliveryInfoRepository.DeleteAsync(userDeliveryInfo.Id, cancellationToken);
        
        _logger.LogInformation("Successfully deleted user delivery info with ID: {Id}", id);
        return userDeliveryInfo.ToGetUserDeliveryInfoDto();
    }
}