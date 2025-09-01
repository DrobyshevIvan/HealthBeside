namespace HealthBeside.Domain.Exceptions;

public class AccountDeletionException(string message) : Exception($"Error while deleting account: {message}");