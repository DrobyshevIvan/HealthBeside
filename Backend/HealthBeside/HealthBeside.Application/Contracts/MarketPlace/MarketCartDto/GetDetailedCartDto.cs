using HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;

namespace HealthBeside.Application.Contracts.MarketPlace.MarketCartDto;

public class GetDetailedCartDto
{
    public Guid Id { get; set; }
    public ICollection<GetCartItemDto> CartItems { get; set; }
    public decimal TotalPrice { get; set; }
}