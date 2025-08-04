using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketReviewRepository : IGenericRepository<MarketReview>
{
    IQueryable<MarketReview> GetQueryable();
    Task<MarketReview?> GetByIdWithAuthorAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MarketReview>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default);
}