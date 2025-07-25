namespace HealthBeside.Application.Contracts.Forum.ForumCommentDto;

public class CreateForumCommentDto
{
    public required string Content { get; init; }
    public required Guid PostId { get; init; }
    public Guid AuthorOfCommentId { get; init; }
    public Guid AuthorOfPostId { get; init; }
}