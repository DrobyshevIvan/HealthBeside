using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class MarketCartRepository : GenericRepository<MarketCart>, IMarketCartRepository
{
    private readonly AppDbContext _context;

    public MarketCartRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<MarketCart?> GetByIdWithAllItems(Guid id)
    {
        return await _context.MarketCarts
            .Include(c => c.CartItems)
            .ThenInclude(c => c.MarketProduct)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<MarketCart?> GetByUserId(Guid id)
    {
        return await _context.MarketCarts
            .Include(c => c.CartItems)
            .ThenInclude(c => c.MarketProduct)
            .FirstOrDefaultAsync(c => c.UserId == id);;
    }
}