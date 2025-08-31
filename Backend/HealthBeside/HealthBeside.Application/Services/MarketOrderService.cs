﻿using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Application.Contracts.User;
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
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class MarketOrderService : IMarketOrderService
{
    private readonly IMarketOrderRepository _marketOrderRepository;
    private readonly IMarketOrderItemRepository _marketOrderItemRepository;
    private readonly IMarketCartRepository _marketCartRepository;
    private readonly IMarketCartItemRepository _marketCartItemRepository;
    private readonly IMarketProductRepository _marketProductRepository;
    private readonly IUserDeliveryInfoRepository _userDeliveryInfoRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<MarketOrderService> _logger;
    private readonly AppDbContext _context;

    public MarketOrderService(IMarketOrderRepository marketOrderRepository,
        IMarketOrderItemRepository marketOrderItemRepository,
        IMarketCartItemRepository marketCartItemRepository,
        IMarketCartRepository marketCartRepository,
        IMarketProductRepository marketProductRepository,
        IUserDeliveryInfoRepository userDeliveryInfoRepository,
        IPaymentRepository paymentRepository,
        ILogger<MarketOrderService> logger,
        AppDbContext context)
    {
        _marketOrderRepository = marketOrderRepository;
        _marketOrderItemRepository = marketOrderItemRepository;
        _marketCartItemRepository = marketCartItemRepository;
        _marketCartRepository = marketCartRepository;
        _marketProductRepository = marketProductRepository;
        _userDeliveryInfoRepository = userDeliveryInfoRepository;
        _paymentRepository = paymentRepository;
        _logger = logger;
        _context = context;
    }

    public async Task<GetOrderDto> GetOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            _logger.LogWarning("Attempt to get order with empty ID");
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));
        }

        _logger.LogInformation("Fetching order {OrderId}", orderId);
        var order = await _marketOrderRepository.GetOrderWithItems(orderId, cancellationToken);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found", orderId);
            throw new MarketOrderException($"Market order with id {orderId} not found");
        }

        _logger.LogInformation("Successfully fetched order {OrderId}", orderId);
        return order.ToGetOrderDto();
    }

    public async Task<IEnumerable<GetOrderDto>> GetAllOrders(
        MarketOrderFilter? marketOrderFilter,
        SortParams? sortParams,
        PageParams? pageParams,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all orders with filters: {@Filter}, sort: {@Sort}, page: {@Page}",
            marketOrderFilter, sortParams, pageParams);

        var query = _marketOrderRepository.GetQueryable();

        if (marketOrderFilter != null)
            query = query.Filter(marketOrderFilter);

        if (sortParams != null)
            query = query.Sort(sortParams);

        if (pageParams != null)
            query = query.Page(pageParams);

        var orders = await query.ToListAsync(cancellationToken);

        _logger.LogInformation("Fetched {Count} orders", orders.Count);
        return orders.Select(o => o.ToGetOrderDto());
    }


    public async Task<IEnumerable<GetOrderDto>> GetAllUserOrders(Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Attempt to get orders with empty userId");
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        }

        _logger.LogInformation("Fetching all orders for user {UserId}", userId);
        var orders = await _marketOrderRepository.GetAllUserOrders(userId, cancellationToken);

        _logger.LogInformation("Fetched {Count} orders for user {UserId}", orders.Count(), userId);
        return orders.Select(o => o.ToGetOrderDto()).ToList();
    }

    public async Task<GetOrderDto> CreateOrder(
        Guid userId,
        UserDeliveryInfoDto deliveryInfoDto,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _logger.LogInformation("Starting order creation for user {UserId}", userId);

            var cart = await _marketCartRepository.GetByUserId(userId, cancellationToken);
            if (cart == null)
                throw new MarketOrderException("Cart not found");

            if (!cart.CartItems.Any())
                throw new MarketOrderException("Cart is empty");

            var existingDeliveryInfo = await _userDeliveryInfoRepository.GetByDetailsAsync(
                userId,
                deliveryInfoDto.City,
                deliveryInfoDto.StreetName,
                deliveryInfoDto.StreetNumber,
                cancellationToken);

            UserDeliveryInfo deliveryInfo;
            if (existingDeliveryInfo is not null)
            {
                _logger.LogInformation("Using existing delivery info {DeliveryInfoId}", existingDeliveryInfo.Id);
                deliveryInfo = existingDeliveryInfo;
            }
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
                _logger.LogInformation("Created new delivery info {DeliveryInfoId}", deliveryInfo.Id);
            }

            foreach (var cartItem in cart.CartItems)
            {
                if (cartItem.MarketProduct.Quantity < cartItem.Quantity)
                    throw new MarketOrderException(
                        $"The amount of desired product '{cartItem.MarketProduct.Name}' is less than the quantity in stock.");
            }

            (string? orderError, MarketOrder? order) = MarketOrder.Create(
                userId,
                cart.CartItems.Sum(i => i.Quantity * i.MarketProduct.Price),
                deliveryInfo.Id);

            if (orderError is not null)
                throw new MarketOrderException(orderError);

            await _marketOrderRepository.AddAsync(order!, cancellationToken);

            foreach (var cartItem in cart.CartItems)
            {
                (string? itemError, MarketOrderItem? orderItem) = MarketOrderItem.Create(
                    cartItem.Quantity,
                    cartItem.MarketProduct.Price,
                    cartItem.ProductId,
                    order!.Id);

                if (itemError is not null)
                    throw new MarketOrderException(itemError);

                if (orderItem is null)
                    throw new MarketOrderException("Unexpected null order item");

                await _marketOrderItemRepository.AddAsync(orderItem!, cancellationToken);

                var product = orderItem.MarketProduct;
                var newStock = product.Quantity - orderItem.Quantity;

                var stockUpdateError = product.UpdateStock(newStock);
                if (stockUpdateError != null)
                    throw new MarketOrderException($"Failed to update stock: {stockUpdateError}");

                await _marketProductRepository.UpdateAsync(product, cancellationToken);
            }

            (string? paymentError, Payment payment) = Payment.Create(userId, order.Id, order.TotalPrice);

            if (paymentError is not null)
                throw new PaymentException(paymentError);

            await _paymentRepository.AddAsync(payment!, cancellationToken);

            var createdOrder = await _marketOrderRepository.GetOrderWithItems(order!.Id, cancellationToken);
            if (createdOrder is null)
                throw new MarketOrderException("Order not found after creation");

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} successfully created for user {UserId}", createdOrder.Id, userId);

            return createdOrder.ToGetOrderDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for user {UserId}", userId);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> CancelOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            _logger.LogWarning("Attempt to cancel order with empty ID");
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));
        }

        _logger.LogInformation("Attempting to cancel order {OrderId}", orderId);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = await _marketOrderRepository.GetOrderWithItems(orderId, cancellationToken);

            if (order is null)
            {
                _logger.LogWarning("Order {OrderId} not found for cancellation", orderId);
                throw new MarketOrderException($"Order with id {orderId} not found");
            }

            if (order.Status != OrderStatus.Pending)
            {
                _logger.LogWarning("Order {OrderId} cannot be cancelled because his status is not pending", orderId);
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
            }

            var payment = await _paymentRepository.GetByOrderId(orderId, cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning("Payment by order id {OrderId} not found", orderId);
                throw new MarketOrderException("Payment not found");
            }

            var paymentStatusError = payment.UpdateStatus(PaymentStatus.Cancelled);
            if (paymentStatusError is not null)
                throw new MarketOrderException(paymentStatusError);

            var orderStatusError = order.UpdateStatus(OrderStatus.Cancelled);
            if (orderStatusError is not null)
                throw new MarketOrderException(orderStatusError);

            await _marketOrderRepository.UpdateAsync(order, cancellationToken);
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} successfully cancelled", orderId);

            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<GetOrderDto> UpdateOrder(Guid orderId, OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            _logger.LogWarning("Attempt to update order with empty ID");
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));
        }

        _logger.LogInformation("Updating status of order {OrderId} to {Status}", orderId, status);
        var order = await _marketOrderRepository.GetOrderWithItems(orderId, cancellationToken);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found for update", orderId);
            throw new MarketOrderException($"Order with id {orderId} not found");
        }

        var error = order.UpdateStatus(status);
        if (error is not null)
        {
            _logger.LogWarning("Failed to update order {OrderId}: {Error}", orderId, error);
            throw new MarketOrderException(error);
        }

        _logger.LogInformation("Successfully updated status of order {OrderId} to {Status}", orderId, status);
        return order.ToGetOrderDto();
    }


    // TODO : Додати Unit Of Work
    public async Task<bool> DeleteOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
        {
            _logger.LogWarning("Attempt to delete order with empty ID");
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));
        }

        _logger.LogInformation("Attempting to delete order {OrderId}", orderId);
        var order = await _marketOrderRepository.GetOrderWithItems(orderId, cancellationToken);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found for deletion", orderId);
            throw new MarketOrderException($"Order with id {orderId} not found");
        }

        if (order.Status == OrderStatus.Delivered)
        {
            _logger.LogWarning("Order {OrderId} cannot be deleted because it is already delivered", orderId);
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
        _logger.LogInformation("Order {OrderId} successfully deleted", orderId);

        return true;
    }

    public async Task CleanupExpiredOrders(CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-15);
        var expiredOrderIds = await _context.MarketOrders
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Pending && o.OrderDate < cutoff)
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);

        foreach (var expiredOrderId in expiredOrderIds)
        {
            try
            {
                await CancelOrder(expiredOrderId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel expired order {OrderId}", expiredOrderId);
            }
        }
    }
}