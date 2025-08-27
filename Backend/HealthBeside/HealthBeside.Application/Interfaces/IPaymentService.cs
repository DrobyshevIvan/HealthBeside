using HealthBeside.Application.Contracts.MarketPlace.Payment;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Interfaces;

public interface IPaymentService
{
    Task ProcessPayment(Guid paymentId, Guid orderId, Guid userId, CancellationToken cancellationToken = default);
    Task<GetDetailedPaymentDto> GetPayment(Guid paymentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GetPaymentDto>> GetPayments(
        PaymentFilter? paymentFilter,
        SortParams? sortParams,
        PageParams? pageParams,
        CancellationToken cancellationToken = default);
}