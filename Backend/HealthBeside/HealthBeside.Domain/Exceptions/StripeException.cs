namespace HealthBeside.Domain.Exceptions;

public class StripeException(string message) : Exception($"Stripe exception: {message}");