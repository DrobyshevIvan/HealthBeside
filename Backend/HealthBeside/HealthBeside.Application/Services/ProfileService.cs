using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.User;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneOf;

namespace HealthBeside.Application.Services;

public class ProfileService :  IProfileService
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly ILogger<ProfileService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(IApplicationUserRepository userRepository,
        ILogger<ProfileService> logger,
        UserManager<ApplicationUser> userManager)
    {
        _userRepository = userRepository;
        _logger = logger;
        _userManager = userManager;
    }
    
    public async Task<bool> UpdateUserInfoAsync(Guid userId, UpdateUserDto userDto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdWithDetailedInfo(userId, cancellationToken);
        
        if(user is null)
            throw new UserNotFound("User not found.");
        
        var result = user.Update(userDto.FirstName, userDto.LastName, userDto.DateOfBirth);
        
        if (result is not null) 
            throw new ApplicationUserException($"Error updating user: {result}");
        
        await _userRepository.UpdateAsync(user, cancellationToken);

        return true;
    }
    
    public async Task<OneOf<GetPatientInfoDto, GetDoctorInfoDto, GetUserInfoDto>> GetUserInfoAsync(
        Guid currentUserId, 
        Guid requestedUserId, 
        CancellationToken cancellationToken = default)
    {
        if (currentUserId != requestedUserId)
        {
            _logger.LogWarning("User {UserId} attempted to access profile of another user {TargetUserId}", currentUserId, requestedUserId);
            throw new UnauthorizedAccessException("You are not authorized to access this user's information.");
        }

        var user = await _userRepository.GetByIdWithDetailedInfo(currentUserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} was not found.", requestedUserId);
            throw new KeyNotFoundException($"User with ID {requestedUserId} not found.");
        }
        
        var roles = await _userManager.GetRolesAsync(user);

        switch (roles.FirstOrDefault())
        {
            case "Patient":
                return new GetPatientInfoDto
                {
                    Id = requestedUserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = "User",
                    DateOfBirth = user.PatientProfile.DateOfBirth,
                    MedicalHistorySummary = user.PatientProfile.MedicalHistorySummary
                };
            case "Admin":
                return new GetUserInfoDto
                {
                    Id = requestedUserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = "Admin"
                };
            case "User":
                return new GetUserInfoDto
                {
                    Id = requestedUserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = "User"
                };
            case "Doctor":
                return new GetDoctorInfoDto
                {
                    Id = requestedUserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = "Doctor",
                    Specialization = user.DoctorProfile.Specialization,
                    MedicalLicenseNumber = user.DoctorProfile.MedicalLicenseNumber,
                    ClinicAffiliation = user.DoctorProfile.ClinicAffiliation,
                    YearsOfExperience = user.DoctorProfile.YearsOfExperience,
                    Education = user.DoctorProfile.Education,
                    Biography = user.DoctorProfile.Biography,
                    Rating = user.DoctorProfile.Rating,
                };
            default:
                throw new RoleNotFound($"Role {roles.FirstOrDefault()} was not found");
        }
    }
}