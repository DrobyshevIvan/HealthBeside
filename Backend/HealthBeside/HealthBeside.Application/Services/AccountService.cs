using HealthBeside.Domain.Contracts;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HealthBeside.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserRepository _userRepository;

    public AccountService(IAuthTokenProcessor authTokenProcessor, UserManager<ApplicationUser> userManager,
        IApplicationUserRepository userRepository)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var userExists = await _userManager.FindByEmailAsync(request.Email) != null;

        if (userExists)
            throw new UserAlreadyExistsException(request.Email);
        
        var (error, user) = ApplicationUser.Create(
            request.FirstName,
            request.LastName,
            request.Email);
        
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.Password); //TODO: doesnt work, maybe cause of ctor
        
        var result = await _userManager.CreateAsync(user);
        
        if(!result.Succeeded)
            throw new UserResistrationFailedException(result.Errors.Select(e => e.Description));
    }

    public async Task LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new LoginFailedException(request.Email);
        
        var (jwtToken, expirationTimeInUtc) = _authTokenProcessor.GenerateJwtToken(user);
        var refreshToken = _authTokenProcessor.GenerateRefreshToken();
        
        var refreshTokenExpirationTokenTimeAtUtc = DateTime.UtcNow.AddDays(7);
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpirationTokenTimeAtUtc;

        await _userManager.UpdateAsync(user);
        
        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationTimeInUtc);
        
        //TODO hide the refresh token from the client side, don't hardcode it 
        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationTokenTimeAtUtc);
    }
    
    //todo make a logout method that clears the cookies

    public async Task RefreshTokenAsync(string? refreshToken)
    {
        if(string.IsNullOrEmpty(refreshToken))
            throw new RefreshTokenException("Refresh token cannot be null or empty.");
        
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
        
        if (user is null)
            throw new RefreshTokenException("Invalid refresh token.");
        
        if(user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            throw new RefreshTokenException("Refresh token has expired.");
    }
}