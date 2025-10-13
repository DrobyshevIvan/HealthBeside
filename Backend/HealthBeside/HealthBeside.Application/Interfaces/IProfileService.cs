using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.User;
using OneOf;

namespace HealthBeside.Application.Interfaces;

public interface IProfileService
{
    Task<bool> UpdateUserInfoAsync(Guid userId, UpdateUserDto userDto,
        CancellationToken cancellationToken = default);
    
    Task<OneOf<GetPatientInfoDto, GetDoctorInfoDto, GetUserInfoDto>> GetUserInfoAsync(
        Guid currentUserId,
        Guid requestedUserId,
        CancellationToken cancellationToken = default);
}