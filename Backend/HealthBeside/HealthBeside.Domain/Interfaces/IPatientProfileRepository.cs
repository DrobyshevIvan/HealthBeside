using HealthBeside.Domain.Models.Users;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IPatientProfileRepository : IGenericRepository<PatientProfile>
{
    Task<PatientProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}