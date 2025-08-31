using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class PatientProfileRepository : GenericRepository<PatientProfile>, IPatientProfileRepository
{
    private readonly AppDbContext _context;

    public PatientProfileRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PatientProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PatientProfiles
            .Include(pp => pp.ApplicationUser)
            .FirstOrDefaultAsync(pp => pp.Id == id, cancellationToken);
    }
    public Task<PatientProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.PatientProfiles
            .Include(pp => pp.ApplicationUser)
            .FirstOrDefaultAsync(pp => pp.ApplicationUserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<PatientProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PatientProfiles
            .Include(pp => pp.ApplicationUser)
            .ToListAsync(cancellationToken);
    }
}