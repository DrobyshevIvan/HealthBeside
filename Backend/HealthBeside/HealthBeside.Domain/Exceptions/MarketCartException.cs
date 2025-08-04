namespace HealthBeside.Domain.Exceptions;

public class MarketCartException(string message) : Exception($"Market cart exception: {message}");