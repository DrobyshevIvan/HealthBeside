using System.Linq.Expressions;
using HealthBeside.Application.Contracts.MarketPlace.Payment;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketOrderDto.Order;
using HealthBeside.Application.Extensions.Mapping.User;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Extensions.Mapping.Marketplace.PaymentDto;

public static class PaymentExtension
{
    public static GetDetailedPaymentDto ToGetDetailedPaymentDto(this Payment payment)
    {
        return new GetDetailedPaymentDto()
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Status = payment.Status.ToString(),
            CreatedAt = payment.CreatedAt,
            StripeCheckoutSessionId = payment.StripeCheckoutSessionId,
            StripePaymentIntentId = payment.StripePaymentIntentId,
            Currency = payment.Currency,
            AmountMinor = payment.AmountMinor,
            ReceiptUrl = payment.ReceiptUrl,
            LatestChargeId = payment.LatestChargeId,
            FailureMessage = payment.FailureMessage,
            FailureCode = payment.FailureCode,
            User = payment.User.ToGetUserDto(),
            Order = payment.Order.ToOrderDtoForPayment()
        };
    }

    public static GetPaymentDto ToGetPaymentDto(this Payment payment)
    {
        return new GetPaymentDto()
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Status = payment.Status.ToString(),
            CreatedAt = payment.CreatedAt,
            UserId = payment.UserId,
            OrderId = payment.OrderId,
        };
    }

    public static IQueryable<Payment> Filter(this IQueryable<Payment> query, PaymentFilter filter)
    {
        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status);

        if (filter.UserId.HasValue)
            query = query.Where(p => p.UserId == filter.UserId);

        if (filter.OrderId.HasValue)
            query = query.Where(p => p.OrderId == filter.OrderId);

        if (filter.MinAmount.HasValue)
            query = query.Where(p => p.Amount >= filter.MinAmount);

        if (filter.MaxAmount.HasValue)
            query = query.Where(p => p.Amount <= filter.MaxAmount);
        
        return query;
    }

    public static IQueryable<Payment> Sort(this IQueryable<Payment> query, SortParams sortParams)
    {
        return sortParams.SortDirection == SortDirection.Descending 
            ? query.OrderByDescending(GetKeySelector(sortParams.OrderBy))
            : query.OrderBy(GetKeySelector(sortParams.OrderBy));
    }

    private static Expression<Func<Payment, object>> GetKeySelector(string? orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
            return p => p.CreatedAt;

        return orderBy switch
        {
            nameof(Payment.Status) => x => x.Status,
            nameof(Payment.CreatedAt) => x => x.CreatedAt,
            nameof(Payment.Amount) => x => x.Amount,
            _ => p => p.CreatedAt,
        };
    }

    public static IQueryable<Payment> Page(this IQueryable<Payment> query, PageParams pageParams)
    {
        var page = pageParams.Page ?? 1;
        var pageSize = pageParams.PageSize ?? 10;

        var skip = (page - 1) * pageSize;
        return query.Skip(skip).Take(pageSize);
    }
}