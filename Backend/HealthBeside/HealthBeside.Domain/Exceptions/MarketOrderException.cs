namespace HealthBeside.Domain.Exceptions;

public class MarketOrderException(string message) : Exception($"Market order exception: {message}");