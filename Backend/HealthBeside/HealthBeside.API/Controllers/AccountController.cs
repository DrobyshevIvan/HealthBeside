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
[Route("api/account/[controller]")]
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
    [Authorize]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        HttpContext httpContext = HttpContext;
        var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];
        await _accountService.RefreshTokenAsync(refreshToken);
        
        return Ok("Refresh token processed successfully.");
    }

    [HttpGet("login/google")]
    public IActionResult GoogleLogin([FromQuery] string returnUrl,
        LinkGenerator linkGenerator,
        SignInManager<ApplicationUser> signInManager,
        HttpContext httpContext)
    {
        var callbackUrl = linkGenerator.GetPathByName(httpContext, "GoogleLoginCallback");
        if (string.IsNullOrWhiteSpace(callbackUrl))
            return BadRequest("Callback route is not configured properly.");

        var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", 
            $"{callbackUrl}?returnUrl={Uri.EscapeDataString(returnUrl)}");

        return Challenge(properties, ["Google"]);
    }

    [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
    public async Task<IActionResult> GoogleCallbackAsync([FromQuery] string returnUrl, HttpContext httpContext,
        IAccountService accountService)
    {
        var result = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Google authentication failed.");
            return Unauthorized();
        }

        await accountService.LoginWithGoogleAsync(result.Principal);

        if (!Url.IsLocalUrl(returnUrl))
            return Redirect("~/");

        return Redirect(returnUrl);
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
    public async Task<IActionResult> GetUserInfoAsync()
    {
        return Ok("User information retrieved successfully.");
    }
}