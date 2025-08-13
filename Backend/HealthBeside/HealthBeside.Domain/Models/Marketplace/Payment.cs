using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Marketplace;

public class Payment
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; }
    public Guid OrderId { get; private set; }
    public MarketOrder Order { get; private set; }
}