namespace HealthBeside.Application.Contracts.Forum.ForumCommentDto;

public class CreateForumCommentDto
{
    public required Guid PostId { get; init; }
    public required string Content { get; init; }
}