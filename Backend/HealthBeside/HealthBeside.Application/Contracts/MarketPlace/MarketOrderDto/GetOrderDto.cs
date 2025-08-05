using HealthBeside.Application.Contracts.MarketPlace.MarketOrderItemDto;
using HealthBeside.Domain.Models.Enums;
using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;

public class GetOrderDto
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public string ShippingAddress { get; set; }
    
    public GetUserDto User { get; set; }
    public ICollection<GetOrderItemDto> MarketOrderItems { get; set; }
}