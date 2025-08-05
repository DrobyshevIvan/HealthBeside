namespace HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;

public class GetCartItemProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string SKU { get; set; }
    public string? ImageUrl { get; set; }
}