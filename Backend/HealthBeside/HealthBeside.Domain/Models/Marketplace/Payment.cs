using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Marketplace;

public class Payment
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public string? StripeCheckoutSessionId { get; private set; }
    public string? StripePaymentIntentId { get; private set; }
    public string? Currency { get; private set; }
    // Сумма в мінімальних одиницях валюти
    public long? AmountMinor {get; private set;}
    public string? ReceiptUrl { get; private set; }
    public string? LatestChargeId { get; private set; }
    public string? FailureMessage { get; private set; }
    public string? FailureCode { get; private set; }
    
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    public Guid OrderId { get; private set; }
    public MarketOrder Order { get; private set; }

    private Payment() {}

    public static (string? Error, Payment Payment) Create(Guid userId, Guid orderId, decimal amount)
    {
        var errors = new List<string>();
        
        if (userId == Guid.Empty)
            errors.Add("User ID cannot be empty");
        
        if (orderId == Guid.Empty)
            errors.Add("Order ID cannot be empty");
        
        if (amount <= 0)
            errors.Add("Amount must be greater than zero");
        
        if (errors.Any())
            return (string.Join("; ", errors), null);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            OrderId = orderId
        };
        
        return (null, payment);
    }

    public string? UpdateStatus(PaymentStatus status)
    {
        if (!Enum.IsDefined(typeof(PaymentStatus), status))
            return "Invalid payment status";

        if (Status == status)
            return "Order already has this status.";
        
        if (!CanUpdateStatus(status))
            return $"Cannot change status from {status} to {status}";
        
        Status = status;
        return null;
    }
    
    private bool CanUpdateStatus(PaymentStatus newStatus)
    {
        return Status switch
        {
            PaymentStatus.Pending => newStatus == PaymentStatus.Completed || newStatus == PaymentStatus.Canceled || newStatus == PaymentStatus.Failed,
            PaymentStatus.Completed => false,
            PaymentStatus.Canceled => false,
            PaymentStatus.Failed => false,
            _ => false
        };
    }

    public string? UpdateStripeAfterCreatingCheckout(string stripeCheckoutSessionId, 
        string currency,
        long amountMinor)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(stripeCheckoutSessionId)) 
            errors.Add("StripeCheckoutSessionId cannot be empty");
        
        if (string.IsNullOrWhiteSpace(currency))
            errors.Add("Currency cannot be empty");
        
        if (amountMinor <= 0) 
            errors.Add("Amount must be greater than zero");
        
        if (errors.Any())
            return string.Join("; ", errors);
        
        StripeCheckoutSessionId = stripeCheckoutSessionId;
        Currency = currency;
        AmountMinor = amountMinor;
        
        return null;
    }

    public string? ApplyStripeSuccess(string paymentIntentId, string? latestChargeId, string? receiptUrl)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(paymentIntentId))
            errors.Add("PaymentIntentId cannot be empty");
        
        if (errors.Any())
            return string.Join("; ", errors);
        
        StripePaymentIntentId = paymentIntentId;
        LatestChargeId = latestChargeId ?? string.Empty;
        ReceiptUrl = receiptUrl ?? string.Empty;
        
        return null;
    }

    public string? ApplyStripeFailure(string paymentIntentId, string? failureCode, string? failureMessage)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(paymentIntentId))
            errors.Add("PaymentIntentId cannot be empty");
        
        if (errors.Any())
            return string.Join("; ", errors);
        
        StripePaymentIntentId = paymentIntentId;
        FailureCode = failureCode ?? null;
        FailureMessage = failureMessage ?? null;
        
        return null;
    }

    public string? ApplyStripeCancelled(string? paymentIntentId)
    {
        StripePaymentIntentId = paymentIntentId;
        
        return null;
    }
}