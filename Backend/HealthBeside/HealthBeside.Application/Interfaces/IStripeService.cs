using HealthBeside.Application.Contracts.MarketPlace.Payment;
using Microsoft.AspNetCore.Http;
using Stripe.Checkout;

namespace HealthBeside.Application.Interfaces;

public interface IStripeService
{
    public Task<string> CreateSession(Guid orderId, Guid userId);
    Task ProcessPayment(string json, HttpRequest request, CancellationToken cancellationToken = default);
}