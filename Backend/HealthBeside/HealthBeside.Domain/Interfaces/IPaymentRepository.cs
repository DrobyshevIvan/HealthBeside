using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetByOrderId(Guid orderId, CancellationToken cancellationToken = default);

    Task<Payment?> GetByCheckoutSessionId(string stripeCheckoutSessionId,
        CancellationToken cancellationToken = default);
}