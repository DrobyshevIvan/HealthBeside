using System.Text.Json.Serialization;
using HealthBeside.Domain.Enums;
using HealthBeside.Infrastructure;

namespace HealthBeside.Application.Contracts.MarketPlace.Payment;

public class GetPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    [JsonConverter(typeof(DateTimeLocalJsonConverter))]
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
}