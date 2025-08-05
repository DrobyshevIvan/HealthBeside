using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Contracts.MarketPlace.MarketOrderItemDto;

public class GetOrderItemDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    public GetProductForOrderItemDto MarketProduct { get; set; }

}