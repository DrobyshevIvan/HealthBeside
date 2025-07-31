namespace HealthBeside.Domain.Exceptions;

public class MarketCategoryException(string message) : Exception($"Market category error: {message}");