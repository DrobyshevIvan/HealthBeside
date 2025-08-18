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
            throw new UserRegistrationFailedException(new[] { error });
        }

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            _logger.LogWarning("User creation failed for email {Email}: {Errors}", request.Email,
                string.Join(", ", errors));
            throw new UserRegistrationFailedException(result.Errors.Select(e => e.Description));
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


//TODO fix roles in DB

//TODO обіграти логіку, аби якщо користувач зареєструвався як юзер, то при записі на консультацію,
//він повинен заповнити всі поля профілю пацієнта, а якщо як лікар, то всі поля профілю лікаря

//////////////////////////////////////////////////////////////
/*public static class UserRoles
{
    public static readonly Guid AdminRoleId = Guid.Parse("aeec2a74-61cd-41eb-8ac7-251c365da8c5");
    public static readonly Guid UserRoleId = Guid.Parse("098f240f-01cf-4925-ba10-4983724f73c3");
    public static readonly Guid DoctorRoleId = Guid.Parse("665c40a7-46ec-4564-a679-755f77c90472");
    public static readonly Guid PatientRoleId = Guid.Parse("c1167d0e-ff05-4df7-bfb5-69baf52c174f");

    public const string Admin = "Admin";
    public const string User = "User";
    public const string Doctor = "Doctor";
    public const string Patient = "Patient";

    public static Dictionary<Guid, string> RoleMapping = new()
    {
        { AdminRoleId, Admin },
        { UserRoleId, User },
        { DoctorRoleId, Doctor },
        { PatientRoleId, Patient }
    };
}

// 2. Оновлений RegisterRequest
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid RoleId { get; set; } // ID ролі, яку обирає користувач

    // Поля для PatientProfile
    public DateTime? DateOfBirth { get; set; }
    public string? MedicalHistorySummary { get; set; }

    // Поля для DoctorProfile
    public string? Specialization { get; set; }
    public string? MedicalLicenseNumber { get; set; }
    public string? ClinicAffiliation { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? Education { get; set; }
    public string? Biography { get; set; }
    public double? Rating { get; set; }
}

public class AccountService : IAccountService
{
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IDoctorProfileRepository _doctorProfileRepository;
    private readonly IPatientProfileRepository _patientProfileRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAuthTokenProcessor authTokenProcessor,
        UserManager<ApplicationUser> userManager,
        IApplicationUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IDoctorProfileRepository doctorProfileRepository,
        IPatientProfileRepository patientProfileRepository,
        ILogger<AccountService> logger)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _doctorProfileRepository = doctorProfileRepository;
        _patientProfileRepository = patientProfileRepository;
        _logger = logger;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Starting registration for email: {Email} with role: {RoleId}", 
            request.Email, request.RoleId);

        // Перевірка чи існує користувач
        var userExists = await _userManager.FindByEmailAsync(request.Email) != null;
        if (userExists)
            throw new UserAlreadyExistsException(request.Email);

        // Валідація ролі
        if (!UserRoles.RoleMapping.ContainsKey(request.RoleId))
        {
            _logger.LogWarning("Invalid role ID provided: {RoleId}", request.RoleId);
            throw new ArgumentException($"Invalid role ID: {request.RoleId}");
        }

        // Перевірка чи користувач намагається зареєструватись як Admin
        if (request.RoleId == UserRoles.AdminRoleId)
        {
            _logger.LogWarning("Attempt to register as Admin blocked for email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Cannot register as Admin through public registration");
        }

        var roleName = UserRoles.RoleMapping[request.RoleId];

        // Створення базового користувача
        var (error, user) = ApplicationUser.Create(request.FirstName, request.LastName, request.Email);
        if (error != null)
        {
            _logger.LogWarning("Failed to create user from request: {Error}", error);
            throw new UserRegistrationFailedException(new[] { error });
        }

        // Створення користувача в Identity
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            _logger.LogWarning("User creation failed for email {Email}: {Errors}", 
                request.Email, string.Join(", ", errors));
            throw new UserRegistrationFailedException(result.Errors.Select(e => e.Description));
        }

        // Надання ролі
        await _userManager.AddToRoleAsync(user, roleName);

        // Створення специфічних профілів залежно від ролі
        await CreateRoleSpecificProfileAsync(user.Id, request, roleName, user);

        _logger.LogInformation("Successfully registered user with ID: {UserId} and role: {Role}", 
            user.Id, roleName);
    }

    private async Task CreateRoleSpecificProfileAsync(Guid userId, RegisterRequest request, string roleName, ApplicationUser user)
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
                // Базовий користувач - нічого додатково не потрібно
                _logger.LogInformation("Created basic user with ID: {UserId}", userId);
                break;
        }
    }

    private async Task CreatePatientProfileAsync(Guid userId, RegisterRequest request, ApplicationUser user)
    {
        // Валідація обов'язкових полів для пацієнта
        if (!request.DateOfBirth.HasValue)
        {
            throw new ArgumentException("Date of birth is required for Patient registration");
        }

        // Використовуємо дефолтне значення для medical history якщо не надано
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
            throw new UserRegistrationFailedException(new[] { error });
        }

        if (patientProfile == null)
        {
            _logger.LogError("Patient profile creation returned null");
            throw new UserRegistrationFailedException(new[] { "Failed to create patient profile" });
        }

        await _patientProfileRepository.AddAsync(patientProfile);
        _logger.LogInformation("Created Patient profile for user: {UserId}", userId);
    }

    private async Task CreateDoctorProfileAsync(Guid userId, RegisterRequest request)
    {
        // Валідація обов'язкових полів для лікаря
        if (string.IsNullOrEmpty(request.Specialization) || 
            string.IsNullOrEmpty(request.MedicalLicenseNumber) ||
            string.IsNullOrEmpty(request.ClinicAffiliation) ||
            string.IsNullOrEmpty(request.Education) ||
            string.IsNullOrEmpty(request.Biography))
        {
            throw new ArgumentException("Specialization, license number, clinic affiliation, education, and biography are required for Doctor registration");
        }

        var (error, doctorProfile) = DoctorProfile.Create(
            userId,
            request.Specialization,
            request.MedicalLicenseNumber,
            request.ClinicAffiliation,
            request.YearsOfExperience ?? 0,
            request.Education,
            request.Biography,
            request.Rating ?? 0.0 // Нові лікарі починають з 0 рейтингом
        );

        if (error != null)
        {
            _logger.LogError("Failed to create doctor profile: {Error}", error);
            throw new UserRegistrationFailedException(new[] { error });
        }

        if (doctorProfile == null)
        {
            _logger.LogError("Doctor profile creation returned null");
            throw new UserRegistrationFailedException(new[] { "Failed to create doctor profile" });
        }

        await _doctorProfileRepository.AddAsync(doctorProfile);
        _logger.LogInformation("Created Doctor profile for user: {UserId}", userId);
    }

    // Метод для створення першого адміна (можна викликати при seed'інгу)
    public async Task CreateInitialAdminAsync(string email, string password, string firstName, string lastName)
    {
        _logger.LogInformation("Creating initial admin user with email: {Email}", email);

        var existingAdmin = await _userManager.FindByEmailAsync(email);
        if (existingAdmin != null)
        {
            _logger.LogInformation("Admin user already exists with email: {Email}", email);
            return;
        }

        var (error, adminUser) = ApplicationUser.Create(firstName, lastName, email);
        if (error != null)
            throw new InvalidOperationException($"Failed to create admin user: {error}");

        var result = await _userManager.CreateAsync(adminUser, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create admin user: {errors}");
        }

        await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
        _logger.LogInformation("Successfully created initial admin user: {UserId}", adminUser.Id);
    }

    // Метод для адміна - призначення ролей іншим користувачам
    public async Task AssignRoleAsync(Guid adminUserId, Guid targetUserId, Guid newRoleId)
    {
        // Перевірка чи поточний користувач - адмін
        var adminUser = await _userManager.FindByIdAsync(adminUserId.ToString());
        if (adminUser == null || !await _userManager.IsInRoleAsync(adminUser, UserRoles.Admin))
        {
            throw new UnauthorizedAccessException("Only admins can assign roles");
        }

        // Перевірка валідності ролі
        if (!UserRoles.RoleMapping.ContainsKey(newRoleId))
        {
            throw new ArgumentException($"Invalid role ID: {newRoleId}");
        }

        var targetUser = await _userManager.FindByIdAsync(targetUserId.ToString());
        if (targetUser == null)
        {
            throw new KeyNotFoundException($"User with ID {targetUserId} not found");
        }

        var newRoleName = UserRoles.RoleMapping[newRoleId];
        
        // Видалення всіх поточних ролей
        var currentRoles = await _userManager.GetRolesAsync(targetUser);
        await _userManager.RemoveFromRolesAsync(targetUser, currentRoles);
        
        // Додавання нової ролі
        await _userManager.AddToRoleAsync(targetUser, newRoleName);

        _logger.LogInformation("Admin {AdminId} assigned role {Role} to user {UserId}", 
            adminUserId, newRoleName, targetUserId);
    }

    // Endpoint для отримання доступних ролей (для фронтенду)
    public List<RoleDto> GetAvailableRoles()
    {
        return UserRoles.RoleMapping
            .Where(r => r.Key != UserRoles.AdminRoleId) // Виключаємо Admin з публічного списку
            .Select(r => new RoleDto 
            { 
                Id = r.Key, 
                Name = r.Value 
            })
            .ToList();
    }

    // Решта методів (GetUserInfoAsync, LoginAsync, RefreshTokenAsync, etc.) залишаються без змін...
}

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// 4. Потрібно також додати репозиторії (якщо їх ще немає)
public interface IDoctorProfileRepository
{
    Task<DoctorProfile> AddAsync(DoctorProfile doctorProfile);
    Task<DoctorProfile?> GetByUserIdAsync(Guid userId);
    // інші методи...
}

public interface IPatientProfileRepository  
{
    Task<PatientProfile> AddAsync(PatientProfile patientProfile);
    Task<PatientProfile?> GetByUserIdAsync(Guid userId);
    // інші методи...
}*/