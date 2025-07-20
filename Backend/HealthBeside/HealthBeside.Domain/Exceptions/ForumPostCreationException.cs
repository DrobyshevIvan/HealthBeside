namespace HealthBeside.Domain.Exceptions;

public class ForumPostCreationException(string message) : Exception($"ForumPost Creation Exception: {message}");