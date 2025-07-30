using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCategoryDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Extensions.Mapping.Marketplace.MarketProductDto;

public static class MarketProductExtension
{
    public static GetDetailedMarketProductDto ToGetDetailedMarketProductDto(this MarketProduct marketProduct)
    {
        return new GetDetailedMarketProductDto
        {
            Id = marketProduct.Id,
            Name = marketProduct.Name,
            Description = marketProduct.Description,
            Price = marketProduct.Price,
            Quantity = marketProduct.Quantity,
            SKU = marketProduct.SKU,
            ImageUrl = marketProduct.ImageUrl,
            Category = marketProduct.Category.ToGetCategoryDto()
        };
    }
}