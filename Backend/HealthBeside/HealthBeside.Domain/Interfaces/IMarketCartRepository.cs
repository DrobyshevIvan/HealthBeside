using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketCartRepository : IGenericRepository<MarketCart>
{
    Task<MarketCart?> GetByIdWithAllItems(Guid id);
    Task<MarketCart?> GetByUserId(Guid id);
}