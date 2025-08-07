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
        int quantity, Guid productId, Guid cartId)
    {
        var errors = new List<string>();
        
        if(quantity <= 0)
            errors.Add("Quantity must be greater than zero.");
        
        if(productId.Equals(Guid.Empty))
            errors.Add("Product Id cannot be empty.");
        
        if(cartId.Equals(Guid.Empty))
            errors.Add("Order Id cannot be empty.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var marketCartItem = new MarketCartItem
        {
            Id = Guid.NewGuid(),
            Quantity = quantity,
            ProductId = productId,
            CartId = cartId,
        };
        
        return (null, marketCartItem);
    }
    
    public string? AddQuantityToExistsItem(int quantity)
    {
        var errors = new List<string>();

        if (quantity <= 0)
            errors.Add("Quantity must be greater than zero.");
        
        if (Quantity + quantity > 100) 
            errors.Add("Quantity exceeds 100.");
        
        if (quantity > 100)
            errors.Add("Quantity cannot be more than 100.");
        
        Quantity += quantity;
        
        if (errors.Any())
            return string.Join("; ", errors);

        return null;
    }

    public string? Update(int? quantity)
    {
        bool hasChanges = false;
        var errors = new List<string>();

        if (quantity.HasValue && quantity != Quantity)
        {
            if (quantity.Value <= 0 || quantity.Value > 100)
                errors.Add("Quantity must be greater than zero and less that hundred.");
            else
            {
                Quantity = quantity.Value;
                hasChanges = true;
            }
        }

        if (errors.Any())
            return string.Join("; ", errors);

        if (!hasChanges)
            return "No valid changes provided.";

        return null;
    }

}