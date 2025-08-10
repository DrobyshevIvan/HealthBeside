using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketOrderRepository : IGenericRepository<MarketOrder>
{
    IQueryable<MarketOrder> GetQueryable();
    Task<MarketOrder?> GetOrderWithItems(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MarketOrder>> GetAllUserOrders(Guid userId, CancellationToken cancellationToken = default);
}