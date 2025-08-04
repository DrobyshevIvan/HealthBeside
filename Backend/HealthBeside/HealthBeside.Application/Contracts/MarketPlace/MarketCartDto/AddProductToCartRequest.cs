namespace HealthBeside.Application.Contracts.MarketPlace.MarketCartDto;

public class AddProductToCartRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}