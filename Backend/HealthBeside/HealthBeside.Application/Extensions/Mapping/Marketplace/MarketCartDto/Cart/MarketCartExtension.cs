using HealthBeside.Application.Contracts.MarketPlace.MarketCartDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCartDto.CartItem;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCartDto.Cart;

public static class MarketCartExtension
{
    public static GetDetailedCartDto ToGetDetailedCartDto(this MarketCart cart)
    {
        return new GetDetailedCartDto
        {
            Id = cart.Id,
            CartItems = cart.CartItems.Select(i => i.ToGetCartItem()).ToList(),
            TotalPrice = cart.CartItems.Select(i => i.MarketProduct.Price * i.Quantity).Sum(),
        };
    }
}