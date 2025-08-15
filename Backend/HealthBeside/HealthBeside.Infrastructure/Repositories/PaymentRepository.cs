using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}