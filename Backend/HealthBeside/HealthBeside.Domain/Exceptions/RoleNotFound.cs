namespace HealthBeside.Domain.Exceptions;

public class RoleNotFound(string message) : Exception($"Role was not found: {message}");