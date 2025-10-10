namespace HealthBeside.Domain.Exceptions;

public class ApplicationUserException(string message) :  Exception($"Application user exception: {message}");