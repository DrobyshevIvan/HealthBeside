using HealthBeside.Domain.Models.Enums;

namespace HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;

public class UpdateOrderRequest
{
    public OrderStatus OrderStatus { get; set; }
}