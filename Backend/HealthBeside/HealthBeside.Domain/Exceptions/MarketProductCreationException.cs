namespace HealthBeside.Domain.Exceptions;

public class MarketProductCreationException(string message) : Exception($"Market product creation error: {message}");