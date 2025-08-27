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

    public IQueryable<Payment> GetQueryable()
    {
        return _context.Payments;
    }

    public async Task<Payment?> GetDetailedPayment(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.Id == paymentId)
            .Include(p => p.Order)
            .Include(p => p.User)
            .FirstOrDefaultAsync(cancellationToken);
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