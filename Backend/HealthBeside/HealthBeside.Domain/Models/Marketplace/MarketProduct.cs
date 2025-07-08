namespace HealthBeside.Domain.Models.Marketplace;

public class MarketProduct
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    
    private MarketProduct() { }

    public static (string? Error, MarketProduct MarketProduct) Create(string name, string description, decimal price)
    {
        var errors = new List<string>();
        
        if(string.IsNullOrWhiteSpace(name))
            errors.Add("Name cannot be empty.");
        
        if(string.IsNullOrWhiteSpace(description))
            errors.Add("Description cannot be empty.");
        
        if(price <= 0)
            errors.Add("Price must be greater than zero.");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);
        
        var marketProduct = new MarketProduct
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price
        };
        
        return (null, marketProduct);
    }
}