using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Infrastructure.Repositories;

public class MarketCategoryRepository : GenericRepository<MarketCategory>, IMarketCategoryRepository
{
    private readonly AppDbContext _context;

    public MarketCategoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}