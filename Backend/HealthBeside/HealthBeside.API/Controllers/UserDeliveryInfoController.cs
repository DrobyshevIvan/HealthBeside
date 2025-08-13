using System.Security.Claims;
using HealthBeside.Application.Contracts.User;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserDeliveryInfoController : ControllerBase
{
    private readonly IUserDeliveryInfoService _userDeliveryInfoService;
    private readonly ILogger<UserDeliveryInfoController> _logger;

    public UserDeliveryInfoController(IUserDeliveryInfoService userDeliveryInfoService,
        ILogger<UserDeliveryInfoController> logger)
    {
        _userDeliveryInfoService = userDeliveryInfoService;
        _logger = logger;
    }

    [HttpGet("get-by-user-id/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserDeliveryInfoByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET request for user delivery info by user ID: {UserId}", userId);
        try
        {
            var userDeliveryInfo = await _userDeliveryInfoService.GetUserDeliveryInfoAsync(userId, cancellationToken);
            _logger.LogInformation("Successfully retrieved user delivery info for user ID: {UserId}", userId);
            return Ok(userDeliveryInfo);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "User delivery info not found for user ID: {UserId}", userId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving user delivery info for user ID: {UserId}", userId);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("get-by-id/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserDeliveryInfoByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to get delivery info. Missing or invalid user ID.");
            return Unauthorized("Invalid or missing user identifier.");
        }
        
        try
        {
            var userDeliveryInfo = await _userDeliveryInfoService.GetUserDeliveryInfoByIdAsync(id, cancellationToken);
            _logger.LogInformation("Successfully retrieved user delivery info by ID: {Id}", id);
            return Ok(userDeliveryInfo);
        }
        catch (UserDeliveryInfoException ex) 
        {
            _logger.LogError(ex, "User delivery info not found for ID: {Id}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving user delivery info by ID: {Id}", id);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("create-user-delivery-info")]
    [Authorize]
    public async Task<IActionResult> CreateUserDeliveryInfoAsync([FromBody] UserDeliveryInfoDto userDeliveryInfoDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("POST request to create user delivery info.");
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to create user delivery info. User ID claim is invalid.");
            return Unauthorized();
        }

        if (userDeliveryInfoDto == null)
        {
            _logger.LogError("User delivery info DTO is null.");
            return BadRequest("User delivery info cannot be null.");
        }

        try
        {
            var userDeliveryInfo = UserDeliveryInfo.Create(
                userDeliveryInfoDto.City,
                userDeliveryInfoDto.PhoneNumber,
                userDeliveryInfoDto.PostalIndex,
                userDeliveryInfoDto.StreetName,
                userDeliveryInfoDto.StreetNumber,
                userId
            );

            var createdUserDeliveryInfo = await _userDeliveryInfoService.CreateUserDeliveryInfoAsync(userDeliveryInfo.UserDeliveryInfo!, cancellationToken);
            
            _logger.LogInformation("Successfully created user delivery info for user ID: {UserId} with ID: {DeliveryInfoId}", userId, createdUserDeliveryInfo.Id);
            return CreatedAtAction(nameof(GetUserDeliveryInfoByIdAsync), new { id = createdUserDeliveryInfo.Id }, createdUserDeliveryInfo);
        }
        catch (UserDeliveryInfoException ex)
        {
            _logger.LogError(ex, "Validation error while creating user delivery info for user ID: {UserId}", userId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating user delivery info for user ID: {UserId}", userId);
            return StatusCode(500, "Internal server error.");
        }
    }
    
    [HttpPut("update-user-delivery-info")]
    [Authorize]
    public async Task<IActionResult> UpdateUserDeliveryInfoAsync([FromBody] UserDeliveryInfoDto userDeliveryInfoDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("PUT request to update user delivery info.");
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to update user delivery info. User ID claim is invalid.");
            return Unauthorized();
        }

        if (userDeliveryInfoDto == null)
        {
            _logger.LogError("User delivery info DTO is null.");
            return BadRequest("User delivery info cannot be null.");
        }

        try
        {
            var userDeliveryInfo = UserDeliveryInfo.Create(
                userDeliveryInfoDto.City,
                userDeliveryInfoDto.PhoneNumber,
                userDeliveryInfoDto.PostalIndex,
                userDeliveryInfoDto.StreetName,
                userDeliveryInfoDto.StreetNumber,
                userId
            );
            
            var updatedUserDeliveryInfo = await _userDeliveryInfoService.UpdateUserDeliveryInfoAsync(userDeliveryInfo.UserDeliveryInfo!, cancellationToken);
            
            _logger.LogInformation("Successfully updated user delivery info for user ID: {UserId}", userId);
            return Ok(updatedUserDeliveryInfo);
        }
        catch (UserDeliveryInfoException ex)
        {
            _logger.LogError(ex, "Error updating user delivery info for user ID: {UserId}. Message: {Message}", userId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating user delivery info for user ID: {UserId}", userId);
            return StatusCode(500, "Internal server error.");
        }
    }
    
    [HttpDelete("delete-user-delivery-info/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserDeliveryInfoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("DELETE request to delete user delivery info with ID: {Id}", id);
        try
        {
            await _userDeliveryInfoService.DeleteUserDeliveryInfoAsync(id, cancellationToken);
            _logger.LogInformation("Successfully deleted user delivery info with ID: {Id}", id);
            return NoContent();
        }
        catch (UserDeliveryInfoException ex)
        {
            _logger.LogError(ex, "User delivery info not found for deletion with ID: {Id}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting user delivery info with ID: {Id}", id);
            return BadRequest(ex.Message);
        }
    }
}
