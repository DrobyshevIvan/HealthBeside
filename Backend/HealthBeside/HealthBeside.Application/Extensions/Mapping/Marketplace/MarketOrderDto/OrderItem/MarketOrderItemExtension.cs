using HealthBeside.Application.Contracts.MarketPlace.MarketOrderItemDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketProductDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Extensions.Mapping.Marketplace.MarketOrderDto.OrderItem;

public static class MarketOrderItemExtension
{
    public static GetOrderItemDto ToGetOrderItem(this MarketOrderItem marketOrderItem)
    {
        return new GetOrderItemDto()
        {
            Id = marketOrderItem.Id,
            Quantity = marketOrderItem.Quantity,
            UnitPrice = marketOrderItem.UnitPrice,
            TotalPrice = marketOrderItem.TotalPrice,
            MarketProduct = marketOrderItem.MarketProduct.ToGetOrderItemProductDto(),
        };
    }
}