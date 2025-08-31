using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Marketplace;

public class Payment
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
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
            PaymentStatus.Pending => newStatus == PaymentStatus.Completed || newStatus == PaymentStatus.Cancelled || newStatus == PaymentStatus.Failed,
            PaymentStatus.Completed => false,
            PaymentStatus.Cancelled => false,
            PaymentStatus.Failed => false,
            _ => false
        };
    }
}