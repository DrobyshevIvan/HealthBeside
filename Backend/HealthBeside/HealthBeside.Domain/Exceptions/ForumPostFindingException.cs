namespace HealthBeside.Domain.Exceptions;

public class ForumPostFindingException(string message) : Exception($"ForumPost Finding Exception: {message}");