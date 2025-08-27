using HealthBeside.Application.Contracts.MarketPlace.Payment;
using HealthBeside.Application.Extensions.Mapping.Marketplace.PaymentDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMarketOrderRepository _marketOrderRepository;
    private readonly IMarketCartRepository _marketCartRepository;
    private readonly IMarketCartItemRepository _marketCartItemRepository;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPaymentRepository paymentRepository,
        IMarketOrderRepository marketOrderRepository,
        IMarketCartRepository marketCartRepository,
        IMarketCartItemRepository marketCartItemRepository,
        ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _marketOrderRepository = marketOrderRepository;
        _marketCartRepository = marketCartRepository;
        _marketCartItemRepository = marketCartItemRepository;
        _logger = logger;
    }

    // TODO : Додати логування
    public async Task ProcessPayment(Guid paymentId, Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _marketOrderRepository.GetOrderWithItems(orderId!, cancellationToken);

        if (order is null)
            throw new PaymentException($"Order with id {orderId} not found");

        var payment = await _paymentRepository.GetAsync(paymentId!, cancellationToken);

        if (payment is null)
            throw new PaymentException($"Payment with id {paymentId} not found");

        // Умовна оплата(поки що)
        Thread.Sleep(3000);

        payment.UpdateStatus(PaymentStatus.Completed);
        order.UpdateStatus(OrderStatus.Confirmed);

        await _paymentRepository.UpdateAsync(payment!, cancellationToken);
        await _marketOrderRepository.UpdateAsync(order!, cancellationToken);
        
        var cart = await _marketCartRepository.GetByUserId(userId!, cancellationToken);

        if (cart is null)
            throw new PaymentException("Cart is not found");
        
        foreach (var cartItem in cart.CartItems)
        {
            await _marketCartItemRepository.DeleteAsync(cartItem.Id, cancellationToken);
        }
    }

    public async Task<GetDetailedPaymentDto> GetPayment(Guid paymentId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching payment {PaymentId}", paymentId);
        var payment = await _paymentRepository.GetDetailedPayment(paymentId, cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Payment with id {PaymentId} not found", paymentId);
            throw new PaymentException($"Payment with id {paymentId} not found");
        }
        
        _logger.LogInformation("Payment with id {PaymentId} retrieved", paymentId);
        return payment.ToGetDetailedPaymentDto();
    }

    public async Task<IEnumerable<GetPaymentDto>> GetPayments(
        PaymentFilter? paymentFilter,
        SortParams? sortParams,
        PageParams? pageParams,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all payments with filters: {@Filter}, sort: {@Sort}, page: {@Page}",
            paymentFilter, sortParams, pageParams);
        
        var query = _paymentRepository.GetQueryable();
        
        if (paymentFilter != null) 
            query = query.Filter(paymentFilter);
        
        if (sortParams != null) 
            query = query.Sort(sortParams);
        
        if (pageParams != null) 
            query = query.Page(pageParams);
        
        var payments = await query.ToListAsync(cancellationToken);
        
        _logger.LogInformation("Fetched {Count} payments", payments.Count);
        return payments.Select(p => p.ToGetPaymentDto());
    }
}