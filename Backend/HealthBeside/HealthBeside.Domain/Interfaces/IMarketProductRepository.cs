using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Models.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketProductRepository : IGenericRepository<MarketProduct>
{
    Task<MarketProduct?> GetByIdWithCategoryAsync(Guid id);
}