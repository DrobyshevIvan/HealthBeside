using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IMarketOrderRepository : IGenericRepository<MarketOrder>
{
    IQueryable<MarketOrder> GetQueryable();
    Task<MarketOrder?> GetOrderWithItems(Guid id);
    Task<IEnumerable<MarketOrder>> GetAllUserOrders(Guid userId);
}