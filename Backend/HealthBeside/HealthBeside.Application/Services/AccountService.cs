using HealthBeside.Application.Contracts;
using HealthBeside.Application.Extensions.Mapping.User;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HealthBeside.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    // private const string LoginProvider = "HealthBesideApi";
    // private const string RefreshToken = "RefreshToken";
    public AccountService(IAuthTokenProcessor authTokenProcessor, 
        UserManager<ApplicationUser> userManager,
        IApplicationUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var userExists = await _userManager.FindByEmailAsync(request.Email) != null;

        if (userExists)
            throw new UserAlreadyExistsException(request.Email);

        var (error, user) = request.ToApplicationUser();
        
        if (error != null)
            throw new UserResistrationFailedException(new[] { error });
        
        var result = await _userManager.CreateAsync(user, request.Password);
        
        if(!result.Succeeded)
            throw new UserResistrationFailedException(result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, "User");
    }

    public async Task LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new LoginFailedException(request.Email);

        (List<Claim> roleClaims, IList<Claim> userClaims) = await GetClaimsForUser(user);
        
        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(user.Id);
        if (storedRefreshToken is not null)
        {
            await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id);
        }
        
        var (jwtToken, expirationTimeInUtc) = _authTokenProcessor.GenerateJwtToken(user, roleClaims, userClaims);
        var refreshTokenString = _authTokenProcessor.GenerateRefreshToken();
        var refreshTokenExpirationTokenTimeAtUtc = DateTime.UtcNow.AddDays(7);

        (string? error, RefreshToken refreshToken) = RefreshToken.Create(refreshTokenString, refreshTokenExpirationTokenTimeAtUtc, user.Id);
        
        if(error != null)
            throw new RefreshTokenException(error);
        
        await _refreshTokenRepository.AddAsync(refreshToken);
        
        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationTimeInUtc);
        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("REFRESH_TOKEN", refreshTokenString, refreshTokenExpirationTokenTimeAtUtc);
    }

    public async Task RefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new RefreshTokenException("Refresh token cannot be null or empty.");

        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

        if (user is null)
            throw new RefreshTokenException("Invalid refresh token.");
        
        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(user.Id);
        
        if(storedRefreshToken.ExpiresOnUtc < DateTime.UtcNow) 
            throw new RefreshTokenException("Refresh token has expired.");

        if (storedRefreshToken.Token != refreshToken)
        {
            throw new RefreshTokenException("Stored refresh token does not match refresh token from user");
        }

        await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id);
        
        (List<Claim> roleClaims, IList<Claim> userClaims) = await GetClaimsForUser(user);
        
        (string jwtToken, DateTime expiresAt) = _authTokenProcessor.GenerateJwtToken(user, roleClaims, userClaims);
        string newRefreshTokenString = _authTokenProcessor.GenerateRefreshToken();
        var newRefreshTokenExpirationTokenTimeAtUtc = DateTime.UtcNow.AddDays(7);
        
        (string? error, RefreshToken newRefreshToken) = RefreshToken.Create(newRefreshTokenString, newRefreshTokenExpirationTokenTimeAtUtc, user.Id);
        
        if(error != null)
            throw new RefreshTokenException(error);
        
        await _refreshTokenRepository.AddAsync(newRefreshToken);
        
        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expiresAt);
        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("REFRESH_TOKEN", newRefreshTokenString, newRefreshTokenExpirationTokenTimeAtUtc);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
        
        if (user is null)
            throw new RefreshTokenException("Invalid refresh token(User not found).");
        
        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(user.Id);
        await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id);
    }

    private async Task<(List<Claim> roleClaim, IList<Claim> userClaim)> GetClaimsForUser(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(user);
        
        return (roleClaims, userClaims);
    }
}