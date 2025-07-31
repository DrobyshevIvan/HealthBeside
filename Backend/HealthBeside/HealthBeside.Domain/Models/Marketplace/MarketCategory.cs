namespace HealthBeside.Domain.Models.Marketplace;

public class MarketCategory
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    public ICollection<MarketProduct> MarketProducts { get; private set; }
    
    private MarketCategory() { }

    public static (string? Error, MarketCategory MarketProduct) Create(string name, string description)
    {
        var errors = new List<string>();
        
        if(string.IsNullOrWhiteSpace(name))
            errors.Add("Name cannot be empty.");
        
        if(string.IsNullOrWhiteSpace(description))
            errors.Add("Description cannot be empty.");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);
        
        var marketCategory = new MarketCategory
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
        
        return (null, marketCategory);
    }

    public string? Update(string? name, string? description)
    {
        var errors = new List<string>();

        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name))
                errors.Add("Name cannot be empty.");
            else
                Name = name;
        }

        if (description is not null)
        {
            if (string.IsNullOrWhiteSpace(description))
                errors.Add("Description cannot be empty.");
            else 
                Description = description;
        }
        
        if (errors.Any())
            return string.Join("; ", errors);

        return null;
    }
}