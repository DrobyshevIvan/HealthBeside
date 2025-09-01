using HealthBeside.Application.Contracts;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using HealthBeside.Application.Contracts.User;
using HealthBeside.Domain.Models.Constants;
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
    private readonly IPatientProfileRepository _patientProfileRepository;
    private readonly IDoctorProfileRepository _doctorProfileRepository;

    public AccountService(IAuthTokenProcessor authTokenProcessor,
        UserManager<ApplicationUser> userManager,
        IApplicationUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AccountService> logger,
        IPatientProfileRepository patientProfileRepository,
        IDoctorProfileRepository doctorProfileRepository)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
        _patientProfileRepository = patientProfileRepository;
        _doctorProfileRepository = doctorProfileRepository;
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

  
    public async Task RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Starting registration for email: {Email}", request.Email);
        var userExists = await _userManager.FindByEmailAsync(request.Email) != null;
        if (userExists)
            throw new UserAlreadyExistsException(request.Email);

        if (!UserRoles.RoleMapping.ContainsKey(request.RoleId))
        {
            _logger.LogWarning("Invalid role ID provided: {RoleId}", request.RoleId);
            throw new ArgumentException($"Invalid role ID: {request.RoleId}");
        }

        if (request.RoleId == UserRoles.AdminRoleId)
        {
            _logger.LogWarning("Attempt to register as Admin blocked for email: {Email}", request.Email);
            throw new UserAlreadyExistsException(request.Email);
        }

        var roleName = UserRoles.RoleMapping[request.RoleId];

        var (error, user) = ApplicationUser.Create(
            request.FirstName,
            request.LastName,
            request.Email
        );

        if (error != null)
        {
            _logger.LogWarning("Failed to create user from request: {Error}", error);
            throw new UserRegistrationFailedException(new[] { error });
        }

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            _logger.LogWarning("User creation failed for email {Email}: {Errors}",
                request.Email, string.Join(", ", errors));
            throw new UserRegistrationFailedException(result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, roleName);

        await CreateRoleSpecificProfileAsync(user.Id, request, roleName, user);

        _logger.LogInformation("User {Email} registered successfully", request.Email);
    }

    private async Task CreateRoleSpecificProfileAsync(
        Guid userId,
        RegisterRequest request,
        string roleName,
        ApplicationUser user)
    {
        switch (roleName)
        {
            case UserRoles.Patient:
                await CreatePatientProfileAsync(userId, request, user);
                break;
            
            case UserRoles.Doctor:
                await CreateDoctorProfileAsync(userId, request);
                break;
            
            case UserRoles.User:
                _logger.LogInformation("User {Email} registered successfully", request.Email);
                break;
        }
    }

    private async Task CreatePatientProfileAsync(Guid userId, RegisterRequest request, ApplicationUser user)
    {
        if (!request.DateOfBirth.HasValue)
        {
            throw new ArgumentException("Date of birth must be provided");
        }
        
        var medicalHistory = string.IsNullOrEmpty(request.MedicalHistorySummary)
            ? "No medical history provided yet."
            : request.MedicalHistorySummary;
        
        var (error, patientProfile) = PatientProfile.Create(
            userId,
            request.DateOfBirth.Value,
            medicalHistory,
            user
        );

        if (error != null)
        {
            _logger.LogError("Failed to create patient profile: {Error}", error);
            throw new UserRegistrationFailedException(new [] { error });
        }

        if (patientProfile is null)
        {
            _logger.LogError("Failed to create patient profile: {Error}", error);
            throw new UserRegistrationFailedException(new [] { "Failed to create patient profile." });
        }

        await _patientProfileRepository.AddAsync(patientProfile);
        _logger.LogInformation("User {Email} registered successfully", request.Email);
    }

    private async Task CreateDoctorProfileAsync(Guid userId, RegisterRequest request)
    {
        var (error, doctorProfile) = DoctorProfile.Create(
            userId,
            request.Specialization,
            request.MedicalLicenseNumber,
            request.ClinicAffiliation,
            request.YearsOfExperience ?? 0,
            request.Education,
            request.Biography,
            request.Rating ?? 0.0);

        if (error != null)
        {
            _logger.LogError("Doctor profile creation returned null");
            throw new UserRegistrationFailedException(new[] { "Failed to create doctor profile" });
        }

        if (doctorProfile == null)
        {
            _logger.LogError("Doctor profile creation returned null");
            throw new UserRegistrationFailedException(new[] { "Failed to create doctor profile" });
        }
        
        await _doctorProfileRepository.AddAsync(doctorProfile);
        _logger.LogInformation("User {Email} registered successfully", request.Email);
    }

    public async Task AssignRoleAsync(Guid adminUserId, Guid targetUserId, Guid newRoleId)
    {
        var adminUser = await _userManager.FindByIdAsync(adminUserId.ToString());
        
        if (adminUser == null || !await _userManager.IsInRoleAsync(adminUser, UserRoles.Admin))
            throw new UnauthorizedAccessException("You are not authorized to assign roles.");
        
        if(!UserRoles.RoleMapping.ContainsKey(newRoleId))
            throw new ArgumentException("Invalid role ID provided.");
        
        var targetUser = await _userManager.FindByIdAsync(targetUserId.ToString());
        
        if(targetUser == null)
            throw new KeyNotFoundException($"User with ID {targetUserId} not found.");
        
        var newRoleName = UserRoles.RoleMapping[newRoleId];
        
        var currentRoles = await _userManager.GetRolesAsync(targetUser);
        await _userManager.RemoveFromRolesAsync(targetUser, currentRoles);

        await _userManager.AddToRoleAsync(targetUser, newRoleName);
        
        _logger.LogInformation("Admin {AdminId} assigned role {Role} to user {UserId}", 
            adminUserId, newRoleName, targetUserId);
    }
    
    public List<RoleDto> GetAvailableRoles()
    {
        return UserRoles.RoleMapping
            .Where(r => r.Key != UserRoles.AdminRoleId) 
            .Select(r => new RoleDto 
            { 
                Id = r.Key, 
                Name = r.Value 
            })
            .ToList();
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

        var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(user.Id, cancellationToken);

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
            
            var roleResult = await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            
            if (!roleResult.Succeeded)
                throw new ExternalLoginProviderException("Google",
                    $"Unable to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

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

    public async Task<bool> DeleteAccountAsync(Guid userId, ClaimsPrincipal currentUser, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        
        if(user == null)
            throw new AccountDeletionException("User not found.");
        
        var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if(currentUserId == null)
            throw new AccountDeletionException("Unable to delete account. Current user not found.");
        
        var isAdmin = currentUser.IsInRole(UserRoles.Admin);
        
        var isOwner = user.Id.ToString() == currentUserId;

        if (!isAdmin && !isOwner)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this account.");
        }
        
        var result = await _userManager.DeleteAsync(user);
        if(!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AccountDeletionException($"Failed to delete account: {errors}");
        }
        _logger.LogInformation("User {UserId} account deleted successfully", userId);
        return true;
    }
}

//TODO обіграти логіку, аби якщо користувач зареєструвався як юзер, то при записі на консультацію,
//він повинен заповнити всі поля профілю пацієнта, а якщо як лікар, то всі поля профілю лікаря
