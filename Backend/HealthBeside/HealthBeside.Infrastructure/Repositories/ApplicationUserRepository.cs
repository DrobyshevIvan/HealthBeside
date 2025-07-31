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
    
    public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var userToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (userToken == null)
            return null;
        
        var user = await _context.Set<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == userToken.UserId);
        return user;
    }
    
    //TODO: Implement other methods as needed, such as GetUserByIdAsync, CreateUserAsync, etc.
}