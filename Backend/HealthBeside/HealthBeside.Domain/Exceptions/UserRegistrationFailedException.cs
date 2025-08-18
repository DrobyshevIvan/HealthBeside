namespace HealthBeside.Domain.Exceptions;

public class UserRegistrationFailedException(IEnumerable<string> errors) 
    : Exception($"Registration failed with errors: {string.Join(", ", errors)}");