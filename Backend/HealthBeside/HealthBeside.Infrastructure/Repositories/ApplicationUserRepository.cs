using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly AppDbContext _context;

    public ApplicationUserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
    
    //TODO: Implement other methods as needed, such as GetUserByIdAsync, CreateUserAsync, etc.
}