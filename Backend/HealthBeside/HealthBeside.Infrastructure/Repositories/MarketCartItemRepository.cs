using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Infrastructure.Repositories;

public class MarketCartItemRepository : GenericRepository<MarketCartItem>, IMarketCartItemRepository
{
    private readonly AppDbContext _context;

    public MarketCartItemRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task DeleteRangeAsync(IEnumerable<MarketCartItem> entities, CancellationToken cancellationToken = default)
    {
        _context.MarketCartItems.RemoveRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
    }
}