using HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketProductDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCartDto.CartItem;

public static class MarketCartItemExtension
{
    public static GetCartItemDto ToGetCartItem(this MarketCartItem marketCartItem)
    {
        return new GetCartItemDto()
        {
            Id = marketCartItem.Id,
            Quantity = marketCartItem.Quantity,
            Product = marketCartItem.MarketProduct.ToGetCartItemProductDto(),
            TotalPrice = marketCartItem.Quantity * marketCartItem.MarketProduct.Price
        };
    }
}