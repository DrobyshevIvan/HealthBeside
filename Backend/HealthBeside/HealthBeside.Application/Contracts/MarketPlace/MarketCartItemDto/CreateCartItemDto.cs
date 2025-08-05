namespace HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;

public class CreateCartItemDto
{
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
}