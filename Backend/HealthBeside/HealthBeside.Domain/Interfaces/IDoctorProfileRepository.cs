using HealthBeside.Domain.Models.Users;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IDoctorProfileRepository : IGenericRepository<DoctorProfile>
{
    Task<DoctorProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorProfile>> GetBySpecializationAsync(
        string specialization, CancellationToken cancellationToken = default);
    Task<DoctorProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorProfile>> GetAllAsync(CancellationToken cancellationToken = default);

}