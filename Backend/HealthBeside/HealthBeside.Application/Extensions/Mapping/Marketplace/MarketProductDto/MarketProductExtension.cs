using System.Linq.Expressions;
using HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;
using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCategoryDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
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

    public static GetCartItemProductDto ToGetCartItemProductDto(this MarketProduct marketProduct)
    {
        return new GetCartItemProductDto()
        {
            Id = marketProduct.Id,
            Name = marketProduct.Name,
            ImageUrl = marketProduct.ImageUrl,
            Price = marketProduct.Price,
            SKU = marketProduct.SKU
        };
    }

    public static IQueryable<MarketProduct> Filter(this IQueryable<MarketProduct> query,
        MarketProductFilter marketProductFilter)
    {
        if (!string.IsNullOrEmpty(marketProductFilter.Name))
            query = query.Where(p => p.Name.ToLower().Contains(marketProductFilter.Name.ToLower()));
        
        if (!string.IsNullOrEmpty(marketProductFilter.SKU))
            query = query.Where(p => p.SKU.ToLower().Contains(marketProductFilter.SKU.ToLower()));
        
        if (marketProductFilter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == marketProductFilter.CategoryId);
        
        if (marketProductFilter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= marketProductFilter.MinPrice);
        
        if (marketProductFilter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= marketProductFilter.MaxPrice);
            
        return query; 
    }

    public static IQueryable<MarketProduct> Sort(this IQueryable<MarketProduct> query, SortParams sortParams)
    {
        return sortParams.SortDirection == SortDirection.Descending
            ? query.OrderByDescending(GetKeySelector(sortParams.OrderBy))
            : query.OrderBy(GetKeySelector(sortParams.OrderBy));
    }

    private static Expression<Func<MarketProduct, object>> GetKeySelector(string? orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
            return x => x.Name;

        return orderBy switch
        {
            nameof(MarketProduct.Description) => x => x.Description,
            nameof(MarketProduct.Price) => x => x.Price,
            nameof(MarketProduct.Reviews) => x => x.Reviews,
            "Rating" => x => x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0,
            _ => x => x.Name
        };
    }

    public static IQueryable<MarketProduct> Page(this IQueryable<MarketProduct> queryable, PageParams pageParams)
    {
        var page = pageParams.Page ?? 1;
        var pageSize = pageParams.PageSize ?? 10;
        
        var skip = (page - 1) * pageSize;
        return queryable.Skip(skip).Take(pageSize);
    }
}