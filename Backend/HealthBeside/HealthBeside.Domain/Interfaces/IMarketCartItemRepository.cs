using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketCartItemRepository : IGenericRepository<MarketCartItem>
{
    Task DeleteRangeAsync(IEnumerable<MarketCartItem> entities, CancellationToken cancellationToken = default);
}