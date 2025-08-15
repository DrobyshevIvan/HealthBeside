using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;

namespace HealthBeside.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMarketOrderRepository _marketOrderRepository;
    private readonly IMarketCartRepository _marketCartRepository;
    private readonly IMarketCartItemRepository _marketCartItemRepository;

    public PaymentService(IPaymentRepository paymentRepository,
        IMarketOrderRepository marketOrderRepository,
        IMarketCartRepository marketCartRepository,
        IMarketCartItemRepository marketCartItemRepository)
    {
        _paymentRepository = paymentRepository;
        _marketOrderRepository = marketOrderRepository;
        _marketCartRepository = marketCartRepository;
        _marketCartItemRepository = marketCartItemRepository;
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
}