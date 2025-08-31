using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Infrastructure.Repositories;

public class DoctorProfileRepository : GenericRepository<DoctorProfile>, IDoctorProfileRepository
{
    private readonly AppDbContext _context;

    public DoctorProfileRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<DoctorProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await _context.DoctorProfiles
            .Include(dp => dp.ApplicationUser)
            .FirstOrDefaultAsync(dp => dp.Id == id, cancellationToken);

        return profile;
    }

    public async Task<DoctorProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _context.DoctorProfiles
            .Include(dp => dp.ApplicationUser)
            .FirstOrDefaultAsync(dp => dp.Id == userId, cancellationToken);

        return profile;
    }

    public async Task<IEnumerable<DoctorProfile>> GetBySpecializationAsync(string specialization, 
        CancellationToken cancellationToken = default)
    {
        var profiles = await  _context.DoctorProfiles
            .Include(dp => dp.ApplicationUser)
            .Where(dp => dp.Specialization.ToLower().Contains(specialization.ToLower()))
            .ToListAsync(cancellationToken);

        return profiles;
    }
    
    public async Task<IEnumerable<DoctorProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await _context.DoctorProfiles
            .Include(dp => dp.ApplicationUser)
            .ToListAsync(cancellationToken);

        return profiles;
    }   
}