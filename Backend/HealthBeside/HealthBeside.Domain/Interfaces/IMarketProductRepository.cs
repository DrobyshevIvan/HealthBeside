using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketProductRepository : IGenericRepository<MarketProduct>
{
    Task<MarketProduct?> GetByIdWithCategoryAsync(Guid id);
    IQueryable<MarketProduct> GetQueryable();
}