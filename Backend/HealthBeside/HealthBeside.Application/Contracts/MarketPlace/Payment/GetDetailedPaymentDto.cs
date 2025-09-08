using System.Text.Json.Serialization;
using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Models.Users;
using HealthBeside.Infrastructure;

namespace HealthBeside.Application.Contracts.MarketPlace.Payment;

public class GetDetailedPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    [JsonConverter(typeof(DateTimeLocalJsonConverter))]
    public DateTime CreatedAt { get; set; }
    
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? Currency { get; set; }
    public long? AmountMinor {get; set;}
    public string? ReceiptUrl { get; set; }
    public string? LatestChargeId { get; set; }
    public string? FailureMessage { get; set; }
    public string? FailureCode { get; set; }
    
    public GetUserDto User { get; set; }
    public OrderDtoForPayment Order { get; set; }
}