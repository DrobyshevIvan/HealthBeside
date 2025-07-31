namespace HealthBeside.Domain.Models.Marketplace;

public class MarketCartItem
{   
    public Guid Id { get; private set; }
    public int Quantity { get; private set; }
    
    public Guid ProductId { get; private set; }
    public MarketProduct MarketProduct { get; private set; }
    
    public Guid CartId { get; private set; }
    public MarketCart MarketCart { get; private set; }
    
    private MarketCartItem() { }

    public static (string? Error, MarketCartItem? MarketCartItem) Create(
        int quantity, Guid productId, Guid orderId)
    {
        var errors = new List<string>();
        
        if(quantity <= 0)
            errors.Add("Quantity must be greater than zero.");
        
        if(productId.Equals(Guid.Empty))
            errors.Add("Product Id cannot be empty.");
        
        if(orderId.Equals(Guid.Empty))
            errors.Add("Order Id cannot be empty.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var marketCartItem = new MarketCartItem
        {
            Id = Guid.NewGuid(),
            Quantity = quantity,
            ProductId = productId,
            CartId = orderId,
        };
        
        return (null, marketCartItem);
    }
}