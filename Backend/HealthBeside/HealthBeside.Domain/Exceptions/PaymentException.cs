namespace HealthBeside.Domain.Exceptions;

public class PaymentException(string error) : Exception($"Payment Error: {error}");