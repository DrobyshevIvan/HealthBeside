using System.Security.Claims;
using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.User;
using HealthBeside.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IProfileService profileService,
        ILogger<ProfileController> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateUserAsync(UpdateUserDto userDto,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier.");
        }
        
        await _profileService.UpdateUserInfoAsync(userId, userDto, cancellationToken);
        return Ok("User data updated successfully");
    }
    
    [HttpGet("get-user-info")]
    [Authorize]
    public async Task<ActionResult<GetUserInfoDto>> GetMyProfile(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid or missing user identifier.");
        }

        var userInfo = await _profileService.GetUserInfoAsync(userId, userId, cancellationToken);
        return Ok(userInfo);
    }
}