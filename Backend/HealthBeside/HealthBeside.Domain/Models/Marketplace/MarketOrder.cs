using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Marketplace;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
public class MarketOrder
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime OrderDate { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string Status { get; private set; }
    public string ShippingAddress { get; private set; }
    
    public ApplicationUser User { get; private set; }
    public MarketProduct Product { get; private set; }
    
    private MarketOrder() { }
    
    public static (string? Error, MarketOrder MarketOrder) Create(Guid userId, Guid productId, int quantity, 
        decimal totalPrice, string shippingAddress)
    {
        var errors = new List<string>();
        
        if(userId == Guid.Empty)
            errors.Add("User ID cannot be empty.");
        
        if(productId == Guid.Empty)
            errors.Add("Product ID cannot be empty.");
        
        if(quantity <= 0)
            errors.Add("Quantity must be greater than zero.");
        
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
            ProductId = productId,
            Quantity = quantity,
            OrderDate = DateTime.UtcNow,
            TotalPrice = totalPrice,
            Status = OrderStatus.Pending.ToString(),
            ShippingAddress = shippingAddress
        };
        
        return (null, marketOrder);
    }
}