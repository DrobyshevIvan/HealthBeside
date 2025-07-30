using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class MarketProductRepository : GenericRepository<MarketProduct>, IMarketProductRepository
{
    private readonly AppDbContext _context;

    public MarketProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<MarketProduct?> GetByIdWithCategoryAsync(Guid id)
    {
        return await _context.MarketProducts
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}