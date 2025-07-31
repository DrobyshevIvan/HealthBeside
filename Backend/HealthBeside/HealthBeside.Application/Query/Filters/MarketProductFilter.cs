namespace HealthBeside.Application.Filters;

public class MarketProductFilter
{
    public string? Name { get; set; }
    public string? SKU { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}