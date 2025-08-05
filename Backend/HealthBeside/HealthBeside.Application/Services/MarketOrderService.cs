using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketOrderDto.Order;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketOrderDto.OrderItem;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Enums;
using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Infrastructure.Repositories;
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
        IMarketProductRepository marketProductRepository
        )
    {
        _marketOrderRepository = marketOrderRepository;
        _marketOrderItemRepository = marketOrderItemRepository;
        _marketCartItemRepository = marketCartItemRepository;
        _marketCartRepository = marketCartRepository;
        _marketProductRepository = marketProductRepository;
    }

    public async Task<GetOrderDto> GetOrder(Guid orderId)
    {
        var order = await _marketOrderRepository.GetOrderWithItems(orderId);

        if (order is null)
            throw new MarketOrderException($"Market order with id {orderId} not found");

        return order.ToGetOrderDto();
    }

    public async Task<IEnumerable<GetOrderDto>> GetAllOrders(MarketOrderFilter? marketOrderFilter,
        SortParams? sortParams,
        PageParams? pageParams)
    {
        var query = _marketOrderRepository.GetQueryable();

        if (marketOrderFilter != null)
            query = query.Filter(marketOrderFilter);
        
        if (sortParams != null)
            query = query.Sort(sortParams);
        
        if (pageParams != null)
            query = query.Page(pageParams);

        var orders = await query.ToListAsync();

        return orders.Select(o => o.ToGetOrderDto());
    }

    public async Task<IEnumerable<GetOrderDto>> GetAllUserOrders(Guid userId)
    {
        var orders = await _marketOrderRepository.GetAllUserOrders(userId);

        return orders.Select(o => o.ToGetOrderDto()).ToList();
    }

    // TODO : Додати до цього метода unitofwork 
    // TODO : Додавання перевірки наявності товару та зміни його при створенні замовлення
    public async Task<GetOrderDto> CreateOrder(Guid userId, string shippingAddress)
    {
        var cart = await _marketCartRepository.GetByUserId(userId);

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
        
        await _marketOrderRepository.AddAsync(order);
        
        foreach (var cartItem in cartItems)
        {
            (error, MarketOrderItem? orderItem) = MarketOrderItem.Create(cartItem.Quantity, cartItem.MarketProduct.Price, cartItem.ProductId, order.Id);

            if (error is not null)
                throw new MarketOrderException(error);

            if (orderItem is null)
                throw new MarketOrderException($"Unknow exception while creating marker order item");

            cartItem.MarketProduct.UpdateStock(cartItem.MarketProduct.Quantity - cartItem.Quantity);
            await _marketProductRepository.UpdateAsync(cartItem.MarketProduct);
            await _marketOrderItemRepository.AddAsync(orderItem);
            await _marketCartItemRepository.DeleteAsync(cartItem.Id);
        }

        order = await _marketOrderRepository.GetOrderWithItems(order.Id);

        if (order is null)
            throw new MarketOrderException($"After creating, order was not found");

        return order.ToGetOrderDto();
    }

    public async Task<GetOrderDto> UpdateOrder(Guid orderId, OrderStatus status)
    {
        var order = await _marketOrderRepository.GetOrderWithItems(orderId);

        if (order is null)
            throw new MarketOrderException($"Order with id {orderId} not found");

        var error = order.UpdateStatus(status);

        if (error is not null)
            throw new MarketOrderException(error);

        return order.ToGetOrderDto();
    }

    // TODO : Додати Unit Of Work
    public async Task<bool> DeleteOrder(Guid orderId)
    {
        var order = await _marketOrderRepository.GetOrderWithItems(orderId);

        if (order is null)
            throw new MarketOrderException($"Order with id {orderId} not found");

        if (order.Status == OrderStatus.Delivered)
        {
            return false;
        }

        foreach (var orderItem in order.MarketOrderItems)
        {
            orderItem.MarketProduct.UpdateStock(orderItem.Quantity + orderItem.MarketProduct.Quantity);

            await _marketProductRepository.UpdateAsync(orderItem.MarketProduct);
            await _marketOrderItemRepository.DeleteAsync(orderItem.Id);
        }

        await _marketOrderRepository.DeleteAsync(orderId);
        return true;
    }
}