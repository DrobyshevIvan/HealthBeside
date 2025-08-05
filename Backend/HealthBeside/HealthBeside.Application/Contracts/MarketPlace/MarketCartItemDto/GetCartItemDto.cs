namespace HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;

public class GetCartItemDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public GetCartItemProductDto Product { get; set; }
    public decimal TotalPrice { get; set; }
}