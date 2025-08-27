using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Domain.Enums;

namespace HealthBeside.Application.Filters;

public class PaymentFilter
{
    public PaymentStatus? Status { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public Guid? UserId { get; set; }
    public Guid? OrderId { get; set; }
}