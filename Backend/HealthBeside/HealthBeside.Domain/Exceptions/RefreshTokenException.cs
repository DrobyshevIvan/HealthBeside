namespace HealthBeside.Domain.Exceptions;

public class RefreshTokenException(string message) : Exception($"Refresh token error: {message}");
