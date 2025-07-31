namespace HealthBeside.Application.Contracts.Forum.ForumCommentDto;

public class UpdateForumCommentDto
{
    public Guid CommentId { get; init; }
    public string Content { get; init; }
    public bool IsAnswer { get; init; } = false;
}