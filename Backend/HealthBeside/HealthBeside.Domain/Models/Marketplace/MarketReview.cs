using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Marketplace;

public class MarketReview
{
    public Guid Id { get; private set; }
    public string Description { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedOn { get; private set; }
    
    public Guid ProductId { get; private set; }
    public MarketProduct MarketProduct { get; private set; }
    
    public string UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    
    private MarketReview() { }

    public static (string? Error, MarketReview? MarketReview) Create(string description, int rating,
        DateTime createdOn, Guid productId,
        string userId)
    {
        var errors = new List<string>();

        if (rating <= 0 || rating > 5)
        {
            errors.Add("Rating must be between 0 and 5.");
        }

        if (productId.Equals(Guid.Empty))
        {
            errors.Add("Product ID cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            errors.Add("User ID cannot be empty.");
        }
        
        if(errors.Any()) 
            return (string.Join("; ", errors), null);

        var marketReview = new MarketReview
        {
            Id = Guid.NewGuid(),
            Description = description,
            Rating = rating,
            CreatedOn = createdOn,
            ProductId = productId,
            UserId = userId
        };
            
        return (null, marketReview);
    }
}