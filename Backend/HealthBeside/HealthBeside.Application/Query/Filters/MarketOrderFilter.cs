using HealthBeside.Domain.Models.Enums;

namespace HealthBeside.Application.Filters;

public class MarketOrderFilter
{
    // public DateTime? OrderDate { get; set; }
    public OrderStatus? Status { get; set; }
    public Guid? UserId { get; set; }
}