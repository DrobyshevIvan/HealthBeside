using HealthBeside.Application.Contracts;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[ApiController]
[Route("api/account/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
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

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        HttpContext httpContext = HttpContext;
        var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];

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