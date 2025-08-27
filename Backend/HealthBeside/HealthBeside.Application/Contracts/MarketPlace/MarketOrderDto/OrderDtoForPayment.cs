namespace HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;

public class OrderDtoForPayment
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
}