namespace HealthBeside.Application.Filters;

public class MarketReviewFilter
{
    public string? Description { get; set; }
    public int? Rating { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? UserId { get; set; }
}