using HealthBeside.Application.Contracts.Forum.ForumPostDto;

namespace HealthBeside.Application.Contracts.Forum.ForumCommentDto;

public class GetDetailedForumCommentDto
{
    public string Content { get; init; }
    public DateTime CreatedAt { get; init; }
    public int Likes { get; init; }
    public int Dislikes { get; init; }
    public GetUserDto Author { get; init; }
    public GetForumPostDto Post { get; init; }
}