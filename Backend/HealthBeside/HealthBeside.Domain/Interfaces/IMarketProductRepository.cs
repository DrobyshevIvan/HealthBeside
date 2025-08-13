using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketProductRepository : IGenericRepository<MarketProduct>
{
    IQueryable<MarketProduct> GetQueryable();
    Task<MarketProduct?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<MarketProduct> entities, CancellationToken cancellationToken = default);
}