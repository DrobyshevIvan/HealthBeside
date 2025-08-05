using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Models.Enums;

namespace HealthBeside.Application.Interfaces;

public interface IMarketOrderService
{
    Task<GetOrderDto> GetOrder(Guid orderId);
    Task<IEnumerable<GetOrderDto>> GetAllOrders(MarketOrderFilter marketOrderFilter,
        SortParams sortParams,
        PageParams pageParams);
    Task<IEnumerable<GetOrderDto>> GetAllUserOrders(Guid userId);
    Task<GetOrderDto> CreateOrder(Guid userId, string shippingAddress);
    Task<GetOrderDto> UpdateOrder(Guid orderId, OrderStatus status);
    Task<bool> DeleteOrder(Guid orderId);
}