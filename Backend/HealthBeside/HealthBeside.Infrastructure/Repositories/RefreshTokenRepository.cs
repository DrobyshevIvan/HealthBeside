using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetRefreshTokenByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}