namespace HealthBeside.Application.Contracts.MarketPlace.Payment;

public class CreateCheckoutSessionDto
{
    public Guid OrderId { get; set; }
}