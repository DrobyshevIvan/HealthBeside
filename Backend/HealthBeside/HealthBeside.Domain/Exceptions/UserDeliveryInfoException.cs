namespace HealthBeside.Domain.Exceptions;

public class UserDeliveryInfoException(string message) : Exception("User delivery info error: " + message);