namespace HealthBeside.Domain.Exceptions;

public class ForumCommentCreationException(string message) : Exception("Forum comment creation failed: " + message);