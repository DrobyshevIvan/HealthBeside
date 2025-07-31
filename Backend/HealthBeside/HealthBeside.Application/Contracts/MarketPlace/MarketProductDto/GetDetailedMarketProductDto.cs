using HealthBeside.Application.Contracts.MarketPlace.MarketCategoryDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;

public class GetDetailedMarketProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string SKU { get; set; }
    public string? ImageUrl { get; set; }
    public GetMarketCategoryDto Category { get; set; }
}