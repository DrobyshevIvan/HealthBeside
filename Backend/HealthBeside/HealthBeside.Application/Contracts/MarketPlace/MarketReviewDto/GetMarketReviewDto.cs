namespace HealthBeside.Application.Contracts.MarketPlace.MarketReviewDto;

public class GetMarketReviewDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedOn { get; set; }
    
    public Guid ProductId { get; set; }
    
    public GetUserDto User { get; set; }
}