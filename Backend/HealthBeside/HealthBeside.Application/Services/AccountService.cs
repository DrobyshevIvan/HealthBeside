using HealthBeside.Application.Contracts;
using HealthBeside.Application.Extensions.Mapping.User;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAuthTokenProcessor authTokenProcessor,
        UserManager<ApplicationUser> userManager,
        IApplicationUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AccountService> logger)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var userExists = await _userManager.FindByEmailAsync(request.Email) != null;
        _logger.LogInformation("Starting registration for email: {Email}", request.Email);
        if (userExists)
            throw new UserAlreadyExistsException(request.Email);

        var (error, user) = request.ToApplicationUser();

        if (error != null)
        {
            _logger.LogWarning("Failed to create user from request: {Error}", error);
            throw new UserResistrationFailedException(new[] { error });
        }

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            _logger.LogWarning("User creation failed for email {Email}: {Errors}", request.Email,
                string.Join(", ", errors));
            throw new UserResistrationFailedException(result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, "User");
        _logger.LogInformation("Successfully registered user with ID: {UserId}", user.Id);
    }

    public async Task LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            _logger.LogWarning("Login failed for email: {Email}", request.Email);
            throw new LoginFailedException(request.Email);
        }

        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(user.Id);
        if (storedRefreshToken is not null)
        {
            await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id);
            _logger.LogDebug("Revoked existing refresh token for user: {UserId}", user.Id);
        }

        await GenerateNewTokensAsync(user);
    }

    public async Task RefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("Refresh token is null or empty");
            throw new RefreshTokenException("Refresh token cannot be null or empty.");
        }

        _logger.LogInformation("Refresh token attempt");
        
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

        if (user is null)
        {
            _logger.LogWarning("Invalid refresh token provided");
            throw new RefreshTokenException("Invalid refresh token.");
        }

        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(user.Id);

        if (storedRefreshToken is null)
        {
            _logger.LogWarning("Stored refresh token not found for user: {UserId}", user.Id);
            throw new RefreshTokenException("Stored refresh token not found.");
        }

        if (storedRefreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token expired for user: {UserId}", user.Id);
            throw new RefreshTokenException("Refresh token has expired.");
        }

        if (storedRefreshToken.Token != refreshToken)
        {
            _logger.LogWarning("Token mismatch for user: {UserId}", user.Id);
            throw new RefreshTokenException("Stored refresh token does not match refresh token from user");
        }

        await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id);

        await GenerateNewTokensAsync(user);
        
        _logger.LogInformation("Successfully refreshed tokens for user: {UserId}", user.Id);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

        if (user is null)
            throw new RefreshTokenException("Invalid refresh token(User not found).");

        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(user.Id);

        if (storedRefreshToken is null)
            throw new RefreshTokenException("Stored refresh token not found.");

        await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id);
    }

    private async Task<(List<Claim> roleClaim, IList<Claim> userClaim)> GetClaimsForUser(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(user);

        return (roleClaims, userClaims);
    }

    private async Task GenerateNewTokensAsync(ApplicationUser user)
    {
        (List<Claim> roleClaims, IList<Claim> userClaims) = await GetClaimsForUser(user);

        (string jwtToken, DateTime expiresAt) = _authTokenProcessor.GenerateJwtToken(user, roleClaims, userClaims);
        string newRefreshTokenString = _authTokenProcessor.GenerateRefreshToken();
        var newRefreshTokenExpirationTokenTimeAtUtc = DateTime.UtcNow.AddDays(7);

        (string? error, RefreshToken? newRefreshToken) = RefreshToken.Create(newRefreshTokenString, newRefreshTokenExpirationTokenTimeAtUtc, user.Id);

        if (error != null)
        {
            _logger.LogError("Failed to create refresh token: {Error}", error);
            throw new RefreshTokenException(error);
        }

        if (newRefreshToken is null)
        {
            _logger.LogError("Failed to create refresh token - null result");
            throw new RefreshTokenException("Failed to create refresh token.");
        }

        var result = await _refreshTokenRepository.AddAsync(newRefreshToken);

        if (result is null)
        {
            _logger.LogError("Failed to store refresh token in database for user: {UserId}", user.Id);
            throw new RefreshTokenException("Failed to create refresh token.");
        }

        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expiresAt);
        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("REFRESH_TOKEN", newRefreshTokenString, newRefreshTokenExpirationTokenTimeAtUtc);
    }
}