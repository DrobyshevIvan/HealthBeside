namespace HealthBeside.Application.Contracts.MarketPlace.MarketReviewDto;

public class CreateMarketReviewDto
{
    public string Description { get; set; }
    public int Rating { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
}