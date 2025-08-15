namespace HealthBeside.Application.Contracts.MarketPlace.Payment;

public class ProcessPaymentRequest
{
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }
}