using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Marketplace;

public class MarketCart
{
    public Guid Id { get; private set; }
    
    public ICollection<MarketCartItem> CartItems { get; private set; }
    
    public string UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    
    private MarketCart() { }

    public static (string? Error, MarketCart? MarketCartItem) Create(
        string userId)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(userId))
            errors.Add("User ID cannot be null or empty");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);

        var marketCart = new MarketCart
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };
        
        return (null, marketCart);
    }
}