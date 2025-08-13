using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Users;
using HealthBeside.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class UserDeliveryInfoRepository : GenericRepository<UserDeliveryInfo>, IUserDeliveryInfoRepository
{
    private readonly AppDbContext _context;

    public UserDeliveryInfoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<UserDeliveryInfo?> GetByDetailsAsync(
        Guid userId,
        string city,
        string streetName,
        int streetNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserDeliveryInfos
            .FirstOrDefaultAsync(u =>
                    u.ApplicationUserId == userId &&
                    u.City == city &&
                    u.StreetName == streetName &&
                    u.StreetNumber == streetNumber,
                cancellationToken);
    }
    
    public async Task<UserDeliveryInfo?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserDeliveryInfos
            .FirstOrDefaultAsync(u => u.ApplicationUserId == userId, cancellationToken);
    }
}