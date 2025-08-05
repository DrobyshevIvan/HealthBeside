using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class MarketOrderRepository : GenericRepository<MarketOrder>, IMarketOrderRepository
{
    private readonly AppDbContext _context;

    public MarketOrderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable<MarketOrder> GetQueryable()
    {
        return _context.MarketOrders
            .Include(o => o.MarketOrderItems)
            .ThenInclude(oi => oi.MarketProduct)
            .Include(o => o.User);
    }

    public async Task<MarketOrder?> GetOrderWithItems(Guid id)
    {
        return await _context.MarketOrders
            .Include(o => o.MarketOrderItems)
            .ThenInclude(oi => oi.MarketProduct)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<MarketOrder>> GetAllUserOrders(Guid userId)
    {
        return await _context.MarketOrders
            .Include(o => o.MarketOrderItems)
            .ThenInclude(oi => oi.MarketProduct)
            .Include(o => o.User)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}