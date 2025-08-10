using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketOrderDto.Order;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Application.Services;

public class MarketOrderService : IMarketOrderService
{
    private readonly IMarketOrderRepository _marketOrderRepository;
    private readonly IMarketOrderItemRepository _marketOrderItemRepository;
    private readonly IMarketCartItemRepository _marketCartItemRepository;
    private readonly IMarketCartRepository _marketCartRepository;
    private readonly IMarketProductRepository _marketProductRepository;

    public MarketOrderService(IMarketOrderRepository marketOrderRepository,
        IMarketOrderItemRepository marketOrderItemRepository,
        IMarketCartItemRepository marketCartItemRepository,
        IMarketCartRepository marketCartRepository,
        IMarketProductRepository marketProductRepository)
    {
        _marketOrderRepository = marketOrderRepository;
        _marketOrderItemRepository = marketOrderItemRepository;
        _marketCartItemRepository = marketCartItemRepository;
        _marketCartRepository = marketCartRepository;
        _marketProductRepository = marketProductRepository;
    }

    public async Task<GetOrderDto> GetOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _marketOrderRepository.GetOrderWithItems(orderId, cancellationToken);

        if (order is null)
            throw new MarketOrderException($"Market order with id {orderId} not found");

        return order.ToGetOrderDto();
    }
    

    public async Task<IEnumerable<GetOrderDto>> GetAllOrders(MarketOrderFilter? marketOrderFilter,
        SortParams? sortParams,
        PageParams? pageParams,
        CancellationToken cancellationToken = default)
    {
        var query = _marketOrderRepository.GetQueryable();

        if (marketOrderFilter != null)
            query = query.Filter(marketOrderFilter);
        
        if (sortParams != null)
            query = query.Sort(sortParams);
        
        if (pageParams != null)
            query = query.Page(pageParams);

        var orders = await query.ToListAsync(cancellationToken);

        return orders.Select(o => o.ToGetOrderDto());
    }

    public async Task<IEnumerable<GetOrderDto>> GetAllUserOrders(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _marketOrderRepository.GetAllUserOrders(userId, cancellationToken);

        return orders.Select(o => o.ToGetOrderDto()).ToList();
    }

    // TODO : Додати до цього метода unitofwork 
    // TODO : Додавання перевірки наявності товару та зміни його при створенні замовлення
    public async Task<GetOrderDto> CreateOrder(Guid userId, string shippingAddress, CancellationToken cancellationToken = default)
    {
        var cart = await _marketCartRepository.GetByUserId(userId, cancellationToken);

        if (cart is null)
            throw new MarketOrderException($"Cart for user with id {userId} not found");

        var cartItems = cart.CartItems;

        (string? error, MarketOrder? order) = MarketOrder.Create(userId,
            cartItems.Sum(i => i.Quantity * i.MarketProduct.Price), shippingAddress);

        if (error is not null)
            throw new MarketOrderException(error);

        if (order is null)
            throw new MarketOrderException($"Unknow exception while creating marker order");
        
        foreach (var cartItem in cartItems)
        {
            if (cartItem.MarketProduct.Quantity < cartItem.Quantity)
                throw new MarketOrderException($"The amount of desired product is less than the quantity in stock");
        }
        
        await _marketOrderRepository.AddAsync(order, cancellationToken);
        
        foreach (var cartItem in cartItems)
        {
            (error, MarketOrderItem? orderItem) = MarketOrderItem.Create(
                cartItem.Quantity, 
                cartItem.MarketProduct.Price, 
                cartItem.ProductId, 
                order.Id);

            if (error is not null)
                throw new MarketOrderException(error);

            if (orderItem is null)
                throw new MarketOrderException($"Unknow exception while creating marker order item");

            cartItem.MarketProduct.UpdateStock(cartItem.MarketProduct.Quantity - cartItem.Quantity);
            await _marketProductRepository.UpdateAsync(cartItem.MarketProduct, cancellationToken);
            await _marketOrderItemRepository.AddAsync(orderItem, cancellationToken);
            await _marketCartItemRepository.DeleteAsync(cartItem.Id, cancellationToken);
        }

        order = await _marketOrderRepository.GetOrderWithItems(order.Id, cancellationToken);

        if (order is null)
            throw new MarketOrderException($"After creating, order was not found");

        return order.ToGetOrderDto();
        
    }

    public async Task<GetOrderDto> UpdateOrder(Guid orderId, OrderStatus status, CancellationToken cancellationToken = default)
    {
        var order = await _marketOrderRepository.GetOrderWithItems(orderId, cancellationToken);

        if (order is null)
            throw new MarketOrderException($"Order with id {orderId} not found");

        var error = order.UpdateStatus(status);

        if (error is not null)
            throw new MarketOrderException(error);

        return order.ToGetOrderDto();
    }

    // TODO : Додати Unit Of Work
    public async Task<bool> DeleteOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _marketOrderRepository.GetOrderWithItems(orderId, cancellationToken);

        if (order is null)
            throw new MarketOrderException($"Order with id {orderId} not found");

        if (order.Status == OrderStatus.Delivered)
        {
            return false;
        }

        foreach (var orderItem in order.MarketOrderItems)
        {
            orderItem.MarketProduct.UpdateStock(orderItem.Quantity + orderItem.MarketProduct.Quantity);

            await _marketProductRepository.UpdateAsync(orderItem.MarketProduct, cancellationToken);
            await _marketOrderItemRepository.DeleteAsync(orderItem.Id, cancellationToken);
        }

        await _marketOrderRepository.DeleteAsync(orderId, cancellationToken);
        return true;
    }
}