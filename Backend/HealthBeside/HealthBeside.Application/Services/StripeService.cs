using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Infrastructure;
using HealthBeside.Infrastructure.Configurations.MarketConfiguration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using StripeException = Stripe.StripeException;

namespace HealthBeside.Application.Services;

public class StripeService : IStripeService
{
    private readonly IConfiguration _configuration;
    private readonly IMarketOrderRepository _marketOrderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOptions<StripeSettings> _stripeSettings;
    private readonly AppDbContext _context;
    private readonly ILogger<StripeService> _logger;
    private readonly IMarketCartRepository _marketCartRepository;
    private readonly IMarketCartItemRepository _marketCartItemRepository;
    private readonly IApplicationUserRepository _userRepository;

    public StripeService(IConfiguration configuration,
        IMarketOrderRepository marketOrderRepository,
        IPaymentRepository paymentRepository,
        IOptions<StripeSettings> stripeSettings,
        AppDbContext context,
        ILogger<StripeService> logger,
        IMarketCartRepository marketCartRepository,
        IMarketCartItemRepository marketCartItemRepository,
        IApplicationUserRepository userRepository)
    {
        _configuration = configuration;
        _marketOrderRepository = marketOrderRepository;
        _paymentRepository = paymentRepository;
        _stripeSettings = stripeSettings;
        _context = context;
        _logger = logger;
        _marketCartRepository = marketCartRepository;
        _marketCartItemRepository = marketCartItemRepository;
        _userRepository = userRepository;
    }
    
    public async Task<string> CreateSession(Guid orderId, Guid userId)
    {
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

        var order = await _marketOrderRepository.GetOrderWithItems(orderId);
        
        if (order is null)
            throw new ApplicationException("Order not found");
        
        var user = await _userRepository.GetAsync(order.UserId);

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = _configuration["Stripe:SuccessUrl"],
            CancelUrl = _configuration["Stripe:CancelUrl"],
            CustomerEmail = user?.Email ?? null,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            Currency = "usd",
            
            Metadata = new Dictionary<string, string>
            {
                ["orderId"] = order.Id.ToString(),
                ["userId"] = userId.ToString()
            },
            PaymentIntentData = new SessionPaymentIntentDataOptions()
            {
                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = order.Id.ToString(),
                    ["userId"] = userId.ToString()
                }
            }
        };

        foreach (var item in order.MarketOrderItems)
        {
            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    UnitAmount = (long)Math.Round(item.UnitPrice * 100m, MidpointRounding.AwayFromZero),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                    {
                        Name = item.MarketProduct.Name
                    }
                },
                Quantity = item.Quantity,
            };
            options.LineItems.Add(sessionLineItem);
        }
        var services = new SessionService();
        Session session = await services.CreateAsync(options);

        var payment = await _paymentRepository.GetByOrderId(order.Id);
        
        if (payment is null)
            throw new ApplicationException("Payment not found");
        
        payment.UpdateStripeAfterCreatingCheckout(session.Id, session.Currency, (long)Math.Round(order.TotalPrice * 100m, MidpointRounding.AwayFromZero));

        await _paymentRepository.UpdateAsync(payment);
        
        return session.Url;
    }

    public async Task ProcessPayment(string json, HttpRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("=== ProcessPayment START ===");
        _logger.LogInformation("Thread ID: {ThreadId}", Thread.CurrentThread.ManagedThreadId);
        
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            request.Headers["Stripe-Signature"],
            _stripeSettings.Value.WebhookSecret
        );
        
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        
        if (paymentIntent is null)
            throw new StripeException("PaymentIntent not found");

        var orderId = Guid.Parse(paymentIntent.Metadata["orderId"]);
        
        var order = await _marketOrderRepository.GetOrderWithItems(orderId, cancellationToken);
            
        if (order is null)
            throw new StripeException("Order not found");
        
        var payment = await _paymentRepository.GetByOrderId(orderId, cancellationToken);
            
        if (payment is null)
            throw new StripeException("Payment not found");
        
        try
        {
            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    _logger.LogInformation("Checkout session completed");
                    break;

                case EventTypes.PaymentIntentSucceeded:
                    {
                        if (payment.Status == PaymentStatus.Completed && order.Status == OrderStatus.Confirmed)
                        {
                            _logger.LogInformation("Payment and Order already completed");
                            break;
                        }
                        
                        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                        try
                        {
                            payment.UpdateStatus(PaymentStatus.Completed);
                
                            var error = payment.ApplyStripeSuccess(paymentIntent.Id, paymentIntent.LatestChargeId, null);
                            if (error != null)
                                throw new StripeException($"Error while applying payment intent info to payment : {error}");
                
                            order.UpdateStatus(OrderStatus.Confirmed);
                
                            var cart = await _marketCartRepository.GetByUserId(order.UserId, cancellationToken);

                            if (cart is null)
                                throw new PaymentException("Cart is not found");
        
                            foreach (var cartItem in cart.CartItems)
                            {
                                await _marketCartItemRepository.DeleteAsync(cartItem.Id, cancellationToken);
                            }
                            
                            await _marketOrderRepository.UpdateAsync(order, cancellationToken);
                            await _paymentRepository.UpdateAsync(payment, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                            throw;
                        }
            
                        await transaction.CommitAsync(cancellationToken);
                        break; 
                    }


                case EventTypes.PaymentIntentCanceled:
                {
                    if (payment.Status == PaymentStatus.Canceled && order.Status == OrderStatus.Canceled)
                    {
                        _logger.LogInformation("Payment and Order already canceled");
                        break;
                    }

                    await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        payment.UpdateStatus(PaymentStatus.Canceled);

                        var error = payment.ApplyStripeCancelled(paymentIntent.Id);
                        if (error != null)
                            throw new StripeException($"Error while applying payment intent info to payment : {error}");

                        order.UpdateStatus(OrderStatus.Canceled);

                        await _marketOrderRepository.UpdateAsync(order, cancellationToken);
                        await _paymentRepository.UpdateAsync(payment, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }

                    await transaction.CommitAsync(cancellationToken);
                    break;
                }
                
                case EventTypes.PaymentIntentPaymentFailed:
                {
                    if (payment.Status == PaymentStatus.Failed)
                    {
                        _logger.LogInformation("Payment already failed");
                        break;
                    }
                    
                    payment.UpdateStatus(PaymentStatus.Failed);
            
                    var error = payment.ApplyStripeFailure(paymentIntent.Id, paymentIntent.LastPaymentError.Code, paymentIntent.LastPaymentError.Message);
                    if (error != null)
                        throw new StripeException($"Error while applying payment intent info to payment : {error}");

                    await _paymentRepository.UpdateAsync(payment, cancellationToken);
                    break; 
                }

                case EventTypes.PaymentIntentCreated:
                    _logger.LogInformation("=== PaymentIntentCreated ===");
                    break;

                case EventTypes.ChargeSucceeded:
                    _logger.LogInformation("=== ChargeSucceeded ===");
                    break;

                case EventTypes.ChargeUpdated:
                    _logger.LogInformation("=== ChargeUpdated ===");
                    break;

                default:
                    _logger.LogInformation("");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event {EventType}", stripeEvent.Type);
            throw;
        }
    }
}