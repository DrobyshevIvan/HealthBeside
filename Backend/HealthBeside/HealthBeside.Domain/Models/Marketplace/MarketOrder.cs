using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Marketplace;

public class MarketOrder
{
    public Guid Id { get; private set; }
    public DateTime OrderDate { get; private set; }
    public decimal TotalPrice { get; private set; }
    public OrderStatus Status { get; private set; }
    
    public Guid UserDeliveryInfoId { get; private set; }
    public UserDeliveryInfo UserDeliveryInfo { get; private set; }
    
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    public Payment? Payment { get; private set; }
    private readonly List<MarketOrderItem> _marketOrderItems = new();
    public ICollection<MarketOrderItem> MarketOrderItems { get; private set; }
    
    private MarketOrder() { }
    
    public static (string? Error, MarketOrder MarketOrder) Create(Guid userId,
        decimal totalPrice, Guid userDeliveryInfoId)
    {
        var errors = new List<string>();
        
        if(userId == Guid.Empty)
            errors.Add("User ID cannot be empty.");
        
        if(totalPrice <= 0)
            errors.Add("Total price must be greater than zero.");

        if(errors.Any())
            return (string.Join("; ", errors), null);
        
        var marketOrder = new MarketOrder
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserDeliveryInfoId = userDeliveryInfoId, 
            OrderDate = DateTime.UtcNow,
            TotalPrice = totalPrice,
            Status = OrderStatus.Pending
        };
        
        return (null, marketOrder);
    }

    public string? UpdateStatus(OrderStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(OrderStatus), newStatus))
            return "Invalid order status.";
        
        if (Status == newStatus)
            return "Order already has this status.";
        
        if (!CanUpdateStatus(newStatus))
            return $"Cannot change status from {Status} to {newStatus}.";
        
        if (Status == OrderStatus.Pending && newStatus == OrderStatus.Confirmed)
        {
            // Можна додати логіку підтвердження замовлення
            // Наприклад, перевірка наявності товарів
        }
    
        Status = newStatus;
        return null; // Успішно оновлено
    }

    private bool CanUpdateStatus(OrderStatus newStatus)
    {
        return Status switch
        {
            OrderStatus.Pending => newStatus == OrderStatus.Confirmed || newStatus == OrderStatus.Cancelled,
            OrderStatus.Confirmed => newStatus == OrderStatus.Shipped || newStatus == OrderStatus.Cancelled,
            OrderStatus.Shipped => newStatus == OrderStatus.Delivered,
            OrderStatus.Delivered => false,
            OrderStatus.Cancelled => false,
            _ => false
        };
    }
    
}