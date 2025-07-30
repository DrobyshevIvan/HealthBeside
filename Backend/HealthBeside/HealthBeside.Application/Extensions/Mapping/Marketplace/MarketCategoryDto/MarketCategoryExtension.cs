using HealthBeside.Application.Contracts.MarketPlace.MarketCategoryDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCategoryDto;

public static class MarketCategoryExtension
{
    public static GetMarketCategoryDto ToGetCategoryDto(this MarketCategory marketCategory)
    {
        return new GetMarketCategoryDto()
        {
            Id = marketCategory.Id,
            Name = marketCategory.Name,
            Description = marketCategory.Description
        };
    }
}