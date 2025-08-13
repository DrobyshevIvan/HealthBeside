using System.Linq.Expressions;
using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketOrderDto.OrderItem;
using HealthBeside.Application.Extensions.Mapping.User;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Models.Users;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace HealthBeside.Application.Extensions.Mapping.Marketplace.MarketOrderDto.Order;

public static class MarketOrderExtension
{
    public static GetOrderDto ToGetOrderDto(this MarketOrder marketOrder)
    {
        return new GetOrderDto()
        {
            Id = marketOrder.Id,
            MarketOrderItems = marketOrder.MarketOrderItems.Select(i => i.ToGetOrderItem()).ToList(),
            OrderDate = marketOrder.OrderDate,
            TotalPrice = marketOrder.TotalPrice,
            Status = marketOrder.Status.ToString(),
            User = marketOrder.User.ToGetUserDto(),
            ShippingAddress = marketOrder.UserDeliveryInfo?.ToGetUserDeliveryInfoDto()
        };
    }
    
    public static GetUserDeliveryInfoDto ToGetUserDeliveryInfoDto(this UserDeliveryInfo deliveryInfo)
    {
        return new GetUserDeliveryInfoDto
        {
            City = deliveryInfo.City,
            PhoneNumber = deliveryInfo.PhoneNumber,
            PostalIndex = deliveryInfo.PostalIndex,
            StreetName = deliveryInfo.StreetName,
            StreetNumber = deliveryInfo.StreetNumber
        };
    }

    public static IQueryable<MarketOrder> Filter(this IQueryable<MarketOrder> query,
        MarketOrderFilter filter)
    {
        if (filter.Status != null)
            query = query.Where(o => o.Status == filter.Status);
        
        if (filter.UserId != null)
            query = query.Where(o => o.UserId == filter.UserId); 
        
        return query;
    }

    public static IQueryable<MarketOrder> Sort(this IQueryable<MarketOrder> query, SortParams sortParams)
    {
        return sortParams.SortDirection == SortDirection.Descending
            ? query.OrderByDescending(GetKeySelector(sortParams.OrderBy))
            : query.OrderBy(GetKeySelector(sortParams.OrderBy));
    }
    
    private static Expression<Func<MarketOrder, object>> GetKeySelector(string? orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
            return x => x.OrderDate;

        return orderBy switch
        {
            nameof(MarketProduct.Description) => x => x.TotalPrice,
            _ => x => x.OrderDate
        };
    }

    public static IQueryable<MarketOrder> Page(this IQueryable<MarketOrder> query, PageParams pageParams)
    {
        var page = pageParams.Page ?? 1;
        var pageSize = pageParams.PageSize ?? 10;
        
        var skip = (page - 1) * pageSize;
        return query.Skip(skip).Take(pageSize);
    }
}