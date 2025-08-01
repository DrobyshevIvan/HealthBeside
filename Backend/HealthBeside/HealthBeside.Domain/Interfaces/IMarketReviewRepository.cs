using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketReviewRepository : IGenericRepository<MarketReview>
{
    IQueryable<MarketReview> GetQueryable();
    Task<MarketReview?> GetByIdWithAuthorAsync(Guid id);
    // Task<IEnumerable<MarketReview?>> GetAllWithAuthorsAsync();
}