using System.Runtime.InteropServices;
using HealthBeside.Application.Contracts;
using HealthBeside.Application.Extensions.Mapping.User;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
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
    
    public async Task<GetUserInfoDto> GetUserInfoAsync(
        Guid currentUserId, 
        Guid requestedUserId, 
        CancellationToken cancellationToken = default)
    {
        if (currentUserId != requestedUserId)
        {
            _logger.LogWarning("User {UserId} attempted to access profile of another user {TargetUserId}", currentUserId, requestedUserId);
            throw new UnauthorizedAccessException("You are not authorized to access this user's information.");
        }

        var user = await _userManager.FindByIdAsync(requestedUserId.ToString());

        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} was not found.", requestedUserId);
            throw new KeyNotFoundException($"User with ID {requestedUserId} not found.");
        }

        return new GetUserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User"
        };
    }

//TODO to make roles dynamic, we can use a configuration file or database to manage roles and permissions
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

    public async Task RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("Refresh token is null or empty");
            throw new RefreshTokenException("Refresh token cannot be null or empty.");
        }

        _logger.LogInformation("Refresh token attempt");
        
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);

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

        await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id, cancellationToken);

        await GenerateNewTokensAsync(user, cancellationToken);
        
        _logger.LogInformation("Successfully refreshed tokens for user: {UserId}", user.Id);
    }

    public async Task LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal, CancellationToken cancellationToken = default)
    {
        if (claimsPrincipal == null)
            throw new ExternalLoginProviderException("Google", "ClaimsPrincipal is null.");

        var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            throw new ExternalLoginProviderException("Google", "Email claim is missing.");

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            var (error, newUser) = ApplicationUser.Create(
                claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? "Unknown",
                claimsPrincipal.FindFirstValue(ClaimTypes.Surname) ?? "Unknown",
                email
            );

            if (error != null || newUser == null)
                throw new ExternalLoginProviderException("Google", $"User creation failed: {error}");

            var result = await _userManager.CreateAsync(newUser);
            if (!result.Succeeded)
                throw new ExternalLoginProviderException("Google",
                    $"Unable to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            user = newUser;
        }

        var loginInfo = new UserLoginInfo("Google", email, "Google");
        var loginResult = await _userManager.AddLoginAsync(user, loginInfo);

        if (!loginResult.Succeeded)
        {
            var existingLogins = await _userManager.GetLoginsAsync(user);
            var alreadyLinked = existingLogins.Any(x => x.LoginProvider == "Google");
            if (!alreadyLinked)
            {
                throw new ExternalLoginProviderException("Google",
                    $"Unable to link Google account: {string.Join(", ", loginResult.Errors.Select(e => e.Description))}");
            }
        }
        try
        {
            await GenerateNewTokensAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ExternalLoginProviderException("Google", $"Token generation failed: {ex.Message}");
        }
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Logout failed: user with refresh token not found");
            throw new RefreshTokenException("Invalid refresh token (user not found).");
        }

        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(user.Id, cancellationToken);

        if (storedRefreshToken is null)
        {
            _logger.LogWarning("Logout failed: no stored refresh token found for user {UserId}", user.Id);
            throw new RefreshTokenException("Refresh token already invalidated or not found.");
        }

        try
        {
            await _refreshTokenRepository.DeleteAsync(storedRefreshToken.Id, cancellationToken);
            _logger.LogInformation("User {UserId} logged out successfully", user.Id);
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogWarning("Refresh token already removed concurrently for user {UserId}", user.Id);
            // Optionally: ignore or rethrow depending on your policy
        }
    }


    private async Task<(List<Claim> roleClaim, IList<Claim> userClaim)> GetClaimsForUser(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(user);

        return (roleClaims, userClaims);
    }

    private async Task GenerateNewTokensAsync(ApplicationUser user, CancellationToken cancellationToken = default)
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

        var result = await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        if (result is null)
        {
            _logger.LogError("Failed to store refresh token in database for user: {UserId}", user.Id);
            throw new RefreshTokenException("Failed to create refresh token.");
        }

        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expiresAt);
        _authTokenProcessor.WriteAuthTokenToHttpOnlyCookie("REFRESH_TOKEN", newRefreshTokenString, newRefreshTokenExpirationTokenTimeAtUtc);
    }
}