namespace HealthBeside.Domain.Exceptions;

public class UserResistrationFailedException(IEnumerable<string> errors) 
    : Exception($"Registration failed with errors: {string.Join(", ", errors)}");