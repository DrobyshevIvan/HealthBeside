using HealthBeside.Application.Contracts.MarketPlace.MarketOrderItemDto;
using HealthBeside.Application.Contracts.User;

namespace HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;

public class GetOrderDto
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public User.GetUserDeliveryInfoDto? ShippingAddress { get; set; }
    
    public GetUserDto User { get; set; }
    public ICollection<GetOrderItemDto> MarketOrderItems { get; set; }
}