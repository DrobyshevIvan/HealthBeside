using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketCartRepository : IGenericRepository<MarketCart>
{
    Task<MarketCart?> GetByIdWithAllItems(Guid id, CancellationToken cancellationToken = default);
    Task<MarketCart?> GetByUserId(Guid id, CancellationToken cancellationToken = default);
}