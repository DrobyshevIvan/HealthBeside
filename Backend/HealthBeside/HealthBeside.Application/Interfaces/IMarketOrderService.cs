using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Enums;

namespace HealthBeside.Application.Interfaces;

public interface IMarketOrderService
{
    Task<GetOrderDto> GetOrder(Guid orderId, CancellationToken cancellationToken);
    Task<IEnumerable<GetOrderDto>> GetAllOrders(
        MarketOrderFilter marketOrderFilter,
        SortParams sortParams,
        PageParams pageParams,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<GetOrderDto>> GetAllUserOrders(Guid userId, CancellationToken cancellationToken = default);
    Task<GetOrderDto> CreateOrder(Guid userId, string shippingAddress, CancellationToken cancellationToken = default);
    Task<GetOrderDto> UpdateOrder(Guid orderId, OrderStatus status, CancellationToken cancellationToken = default);
    Task<bool> DeleteOrder(Guid orderId, CancellationToken cancellationToken = default);
}