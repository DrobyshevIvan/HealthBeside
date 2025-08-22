using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByOrderId(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
    }

    public async Task<Payment?> GetByCheckoutSessionId(string stripeCheckoutSessionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.StripeCheckoutSessionId == stripeCheckoutSessionId, cancellationToken);
    }
}