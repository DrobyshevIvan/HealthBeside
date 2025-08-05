namespace HealthBeside.Application.Contracts.MarketPlace.MarketOrderItemDto;

public class GetProductForOrderItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SKU { get; set; }
    public string? ImageUrl { get; set; }
}