using HealthBeside.Domain.Enums;

namespace HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;

public class UpdateOrderRequest
{
    public OrderStatus OrderStatus { get; set; }
}