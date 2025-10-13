using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
{
    private readonly AppDbContext _context;

    public ApplicationUserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var userToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        if (userToken == null)
            return null;
        
        var user = await _context.Set<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == userToken.UserId, cancellationToken);
        return user;
    }

    public async Task<ApplicationUser?> GetByIdWithDetailedInfo(Guid id, CancellationToken cancellationToken)
    {
        var result = await _context.Users
            .Include(u => u.PatientProfile)
            .Include(u => u.DoctorProfile)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return result;
    }
}