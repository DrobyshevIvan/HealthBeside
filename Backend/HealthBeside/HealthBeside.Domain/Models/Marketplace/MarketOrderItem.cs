namespace HealthBeside.Domain.Models.Marketplace;

public class MarketOrderItem
{
    public Guid Id { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    
    public Guid ProductId { get; private set; }
    public MarketProduct MarketProduct { get; private set; }
    
    public Guid OrderId { get; private set; }
    public MarketOrder MarketOrder { get; private set; }
    
    private MarketOrderItem() { }

    public static (string? Error, MarketOrderItem? MarketOrderItem) Create(
        int quantity, decimal unitPrice, Guid productId, Guid orderId)
    {
        var errors = new List<string>();
        
        if (quantity <= 0)
            errors.Add("Quantity must be greater than zero.");
        
        if (unitPrice <= 0)
            errors.Add("Unit price must be greater than zero.");
        
        if (productId.Equals(Guid.Empty))
            errors.Add("Product id cannot be empty.");
        
        if (orderId.Equals(Guid.Empty))
            errors.Add("Order id cannot be empty.");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);

        var marketOrderItem = new MarketOrderItem
        {
            Id = Guid.NewGuid(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = quantity * unitPrice,
            ProductId = productId,
            OrderId = orderId
        };
        
        return (null, marketOrderItem);
    }
}