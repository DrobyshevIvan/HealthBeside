using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class MarketReviewRepository : GenericRepository<MarketReview>, IMarketReviewRepository
{
    private readonly AppDbContext _context;

    public MarketReviewRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable<MarketReview> GetQueryable()
    {
        return _context.MarketReviews
            .Include(r => r.User);
    }
    
    public async Task<MarketReview?> GetByIdWithAuthorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MarketReviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MarketReview>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MarketReviews
            .AsNoTracking()
            .Include(r => r.User)
            .ToListAsync(cancellationToken);
    }

}