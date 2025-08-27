using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    IQueryable<Payment> GetQueryable();
    Task<Payment?> GetDetailedPayment(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByOrderId(Guid orderId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByCheckoutSessionId(string stripeCheckoutSessionId,
        CancellationToken cancellationToken = default);
}