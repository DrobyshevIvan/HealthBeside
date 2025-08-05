using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Infrastructure.Repositories;

public class MarketOrderItemRepository : GenericRepository<MarketOrderItem>, IMarketOrderItemRepository
{
    private readonly AppDbContext _context;

    public MarketOrderItemRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}