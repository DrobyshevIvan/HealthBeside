using HealthBeside.Domain.Models.Enums;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Marketplace;

public class MarketOrder
{
    public Guid Id { get; private set; }
    public DateTime OrderDate { get; private set; }
    public decimal TotalPrice { get; private set; }
    public OrderStatus Status { get; private set; }
    public string ShippingAddress { get; private set; }
    
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    
    public ICollection<MarketOrderItem> MarketOrderItems { get; private set; }
    
    private MarketOrder() { }
    
    public static (string? Error, MarketOrder MarketOrder) Create(Guid userId,
        decimal totalPrice, string shippingAddress)
    {
        var errors = new List<string>();
        
        if(userId == Guid.Empty)
            errors.Add("User ID cannot be empty.");
        
        if(totalPrice <= 0)
            errors.Add("Total price must be greater than zero.");
        
        if(string.IsNullOrWhiteSpace(shippingAddress))
            errors.Add("Shipping address cannot be empty.");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);
        
        var marketOrder = new MarketOrder
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalPrice = totalPrice,
            Status = OrderStatus.Pending,
            ShippingAddress = shippingAddress
        };
        
        return (null, marketOrder);
    }
}