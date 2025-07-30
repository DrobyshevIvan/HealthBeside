namespace HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;

public class UpdateMarketProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Quantity { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? CategoryId { get; set; }
}