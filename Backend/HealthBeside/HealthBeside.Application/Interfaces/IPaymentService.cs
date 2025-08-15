using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Interfaces;

public interface IPaymentService
{
    Task ProcessPayment(Guid paymentId, Guid orderId, Guid userId, CancellationToken cancellationToken = default);
}