using System.Linq.Expressions;
using HealthBeside.Application.Contracts.MarketPlace.MarketReviewDto;
using HealthBeside.Application.Extensions.Mapping.User;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Extensions.Mapping.Marketplace.MarketReviewDto;

public static class MarketReviewExtension
{
    public static GetMarketReviewDto ToGetMarketReviewDto(this MarketReview marketReview)
    {
        return new GetMarketReviewDto
        {
            Id = marketReview.Id,
            ProductId = marketReview.ProductId,
            Description = marketReview.Description,
            CreatedOn = marketReview.CreatedOn,
            Rating = marketReview.Rating,
            User = marketReview.User.ToGetUserDto(),
        };
    }

    public static IQueryable<MarketReview> Filter(this IQueryable<MarketReview> query, MarketReviewFilter filter)
    {
        if (filter.Description != null)
            query = query.Where(r => r.Description.ToLower().Contains(filter.Description.ToLower()));
        
        if (filter.Rating != null)
            query = query.Where(r => r.Rating == filter.Rating);
        
        if (filter.UserId != null)
            query = query.Where(r => r.UserId == filter.UserId);
        
        if (filter.ProductId != null)
            query = query.Where(r => r.ProductId == filter.ProductId);
        
        return query;
    }

    public static IQueryable<MarketReview> Sort(this IQueryable<MarketReview> query, SortParams sortParams)
    {
        return sortParams.SortDirection == SortDirection.Descending 
            ? query.OrderByDescending(GetKeySelector(sortParams.OrderBy))
            : query.OrderBy(GetKeySelector(sortParams.OrderBy));
    }

    private static Expression<Func<MarketReview, object>> GetKeySelector(string? orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
            return x => x.Rating;

        return orderBy switch
        {
            nameof(MarketReview.Description) => x => x.Description,
            nameof(MarketReview.CreatedOn) => x => x.CreatedOn,
            nameof(MarketReview.Rating) => x => x.Rating,
            _ => x => x.Rating
        };
    }

    public static IQueryable<MarketReview> Page(this IQueryable<MarketReview> queryable, PageParams pageParams)
    {
        var page = pageParams.Page ?? 1;
        var pageSize = pageParams.PageSize ?? 10;
        
        var skip = (page - 1) * pageSize;
        return queryable.Skip(skip).Take(pageSize);
    }
}