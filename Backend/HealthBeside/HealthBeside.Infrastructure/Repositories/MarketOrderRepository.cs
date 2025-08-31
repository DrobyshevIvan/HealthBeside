using HealthBeside.Domain.Enums;
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

    public async Task<MarketOrder?> GetOrderWithItems(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MarketOrders
            .Include(o => o.MarketOrderItems)
            .ThenInclude(oi => oi.MarketProduct)
            .Include(o => o.UserDeliveryInfo)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken); 
    }

    public async Task<IEnumerable<MarketOrder>> GetAllUserOrders(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.MarketOrders
            .Include(o => o.MarketOrderItems)
            .ThenInclude(oi => oi.MarketProduct)
            .Include(o => o.User)
            .Where(o => o.UserId == userId)
            .Include(udi => udi.UserDeliveryInfo)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<MarketOrder?> GetNearestOrderOlderThan(DateTime time, OrderStatus orderStatus,
        CancellationToken cancellationToken = default)
    {
        return await _context.MarketOrders
            .AsNoTracking()
            .Where(o => o.OrderDate > time && o.Status == orderStatus)
            .OrderBy(o => o.OrderDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
}