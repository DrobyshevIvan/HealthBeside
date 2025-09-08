using System.Security.Claims;
using HealthBeside.Application.Contracts;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        await _accountService.RegisterAsync(request);
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        await _accountService.LoginAsync(request);
        return Ok("User logged in successfully.");
    }
    

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        HttpContext httpContext = HttpContext;
        var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];
        await _accountService.RefreshTokenAsync(refreshToken);
        
        return Ok("Refresh token processed successfully.");
    }

    [HttpGet("login/google")]
    public IActionResult GoogleLogin([FromQuery] string returnUrl, [FromServices] LinkGenerator linkGenerator)
    {
        var callbackUrl = linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback");

        if (string.IsNullOrWhiteSpace(callbackUrl))
            return BadRequest("Callback route is not configured properly.");

        var properties = new AuthenticationProperties
        {
            RedirectUri = $"{callbackUrl}?returnUrl={Uri.EscapeDataString(returnUrl)}"
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }


    [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
    public async Task<IActionResult> GoogleCallbackAsync(
        [FromQuery] string returnUrl, 
        [FromServices] IAccountService accountService)
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
        {
            _logger.LogWarning("Google authentication failed.");
            return Unauthorized();
        }

        await accountService.LoginWithGoogleAsync(result.Principal);

        return Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : Redirect("~/");
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        var refreshToken = HttpContext.Request.Cookies["REFRESH_TOKEN"];

        if (refreshToken is null)
            throw new RefreshTokenException("Refresh token was not found in cookies");
        
        await _accountService.LogoutAsync(refreshToken);
        
        Response.Cookies.Delete("ACCESS_TOKEN");
        Response.Cookies.Delete("REFRESH_TOKEN");
        
        return Ok("Logged out successfully.");
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

        var userInfo = await _accountService.GetUserInfoAsync(userId, userId, cancellationToken);
        return Ok(userInfo);
    }

    [HttpDelete("delete-account")]
    [Authorize]
    public async Task<IActionResult> DeleteAccountAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _accountService.DeleteAccountAsync(userId, User, cancellationToken);
            if (result)
                return NoContent();

            return BadRequest("Account deletion failed.");
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message); 
        }
        catch (AccountDeletionException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = "Internal server error", details = e.Message });
        }
    }

}