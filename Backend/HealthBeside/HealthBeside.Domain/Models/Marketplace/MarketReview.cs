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
    
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    
    private MarketReview() { }

    public static (string? Error, MarketReview? MarketReview) Create(string description, int rating, Guid productId, Guid userId)
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

        if (userId == Guid.Empty)
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
            CreatedOn = DateTime.UtcNow,
            ProductId = productId,
            UserId = userId
        };
            
        return (null, marketReview);
    }

    public string? Update(string? description, int? rating)
    {
        var errors = new List<string>();

        if (description is not null)
        {
            if (string.IsNullOrWhiteSpace(description))
                errors.Add("Description cannot be empty.");
            else
                Description = description;
        }

        if (rating.HasValue)
        {
            if (rating.Value <= 0 && rating.Value > 5) 
                errors.Add("Rating must be between 0 and 5.");
            else 
                Rating = rating.Value;
        }
        
        if (errors.Any())
            return string.Join("; ", errors);
        
        return null;
    }
}