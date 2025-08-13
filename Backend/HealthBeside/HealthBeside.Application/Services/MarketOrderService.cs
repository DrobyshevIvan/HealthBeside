using HealthBeside.Application.Contracts;
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
using HealthBeside.Domain.Models.Users;
using HealthBeside.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Application.Services;

public class MarketOrderService : IMarketOrderService
{
    private readonly IMarketOrderRepository _marketOrderRepository;
    private readonly IMarketOrderItemRepository _marketOrderItemRepository;
    private readonly IMarketCartItemRepository _marketCartItemRepository;
    private readonly IMarketCartRepository _marketCartRepository;
    private readonly IMarketProductRepository _marketProductRepository;
    private readonly IUserDeliveryInfoRepository _userDeliveryInfoRepository;
    private readonly AppDbContext _context;

    public MarketOrderService(IMarketOrderRepository marketOrderRepository,
        IMarketOrderItemRepository marketOrderItemRepository,
        IMarketCartItemRepository marketCartItemRepository,
        IMarketCartRepository marketCartRepository,
        IMarketProductRepository marketProductRepository,
        IUserDeliveryInfoRepository userDeliveryInfoRepository,
        AppDbContext context)
    {
        _marketOrderRepository = marketOrderRepository;
        _marketOrderItemRepository = marketOrderItemRepository;
        _marketCartItemRepository = marketCartItemRepository;
        _marketCartRepository = marketCartRepository;
        _marketProductRepository = marketProductRepository;
        _userDeliveryInfoRepository = userDeliveryInfoRepository;
        _context = context;
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
    public async Task<GetOrderDto> CreateOrder(
        Guid userId, 
        UserDeliveryInfoDto deliveryInfoDto, 
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var cart = await _marketCartRepository.GetByUserId(userId, cancellationToken);
            if(cart == null)
                throw new MarketOrderException("Cart not found");

            UserDeliveryInfo deliveryInfo;
            
            var existingDeliveryInfo = await _userDeliveryInfoRepository.GetByDetailsAsync( //TODO
                userId, 
                deliveryInfoDto.City, 
                deliveryInfoDto.StreetName, 
                deliveryInfoDto.StreetNumber, 
                cancellationToken);
            
            if(existingDeliveryInfo is not null)
                deliveryInfo = existingDeliveryInfo;
            else
            {
                (string? createError, UserDeliveryInfo? newDeliveryInfo) = UserDeliveryInfo.Create(
                    deliveryInfoDto.City,
                    deliveryInfoDto.PhoneNumber,
                    deliveryInfoDto.PostalIndex,
                    deliveryInfoDto.StreetName,
                    deliveryInfoDto.StreetNumber,
                    userId);

                if (createError is not null)
                    throw new MarketOrderException(createError);
            
                deliveryInfo = newDeliveryInfo!;
                await _userDeliveryInfoRepository.AddAsync(deliveryInfo, cancellationToken);
            }
            
            var cartItems = cart.CartItems;
            
            if (!cartItems.Any())
                throw new MarketOrderException("Cart is empty");

            foreach (var cartItem in cartItems)
            {
                if (cartItem.MarketProduct.Quantity < cartItem.Quantity)
                    throw new MarketOrderException(
                        $"The amount of desired product '{cartItem.MarketProduct.Name}' is less than the quantity in stock.");
            }
            
            (string? error, MarketOrder? order) = MarketOrder.Create(
                userId,
                cartItems.Sum(i => i.Quantity * i.MarketProduct.Price),
                deliveryInfo.Id); 

            if (error is not null)
                throw new MarketOrderException(error);
            
            var orderItems = new List<MarketOrderItem>();
            foreach (var cartItem in cartItems)
            {
                (string? itemError, MarketOrderItem? orderItem) = MarketOrderItem.Create(
                    cartItem.Quantity,
                    cartItem.MarketProduct.Price,
                    cartItem.ProductId,
                    order.Id);

                if (itemError is not null)
                    throw new MarketOrderException(itemError);

                orderItems.Add(orderItem!);

                cartItem.MarketProduct.UpdateStock(cartItem.MarketProduct.Quantity - cartItem.Quantity);
            }

            order.AddOrderItems(orderItems);
            await _marketOrderRepository.AddAsync(order, cancellationToken);
            await _marketProductRepository.UpdateRangeAsync(cartItems.Select(ci => ci.MarketProduct), cancellationToken);
            await _marketCartItemRepository.DeleteRangeAsync(cartItems, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return order.ToGetOrderDto();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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
            var product = orderItem.MarketProduct;
            var newStock = product.Quantity + orderItem.Quantity;
            
            var stockUpdateError = product.UpdateStock(newStock);
            if (stockUpdateError != null)
                throw new MarketOrderException($"Failed to update stock: {stockUpdateError}");

            await _marketProductRepository.UpdateAsync(product, cancellationToken);
            await _marketOrderItemRepository.DeleteAsync(orderItem.Id, cancellationToken);
        }

        await _marketOrderRepository.DeleteAsync(orderId, cancellationToken);
        return true;
    }
}