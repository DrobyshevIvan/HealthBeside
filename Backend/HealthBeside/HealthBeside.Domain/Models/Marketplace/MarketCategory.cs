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
        bool hasChanges = false;
        var errors = new List<string>();

        if(!string.IsNullOrWhiteSpace(name) && Name != name)
        {
            if (string.IsNullOrWhiteSpace(name))
                errors.Add("Name cannot be empty.");
            else
            {
                Name = name;
                hasChanges = true;
            }
        }

        if(!string.IsNullOrWhiteSpace(description) && Description != description)
        {
            if (string.IsNullOrWhiteSpace(description))
                errors.Add("Description cannot be empty.");
            else
            {
                Description = description;
                hasChanges = true;
            }
        }
        
        if (errors.Any())
            return string.Join("; ", errors);

        if(!hasChanges)
            return "No changes made.";
        
        return null;
    }
}