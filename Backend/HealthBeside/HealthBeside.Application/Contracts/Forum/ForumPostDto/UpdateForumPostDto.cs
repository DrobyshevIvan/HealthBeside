namespace HealthBeside.Application.Contracts.Forum.ForumPostDto;

public class UpdateForumPostDto
{
    public Guid PostId { get; init; }
    public string Title { get; init; } 
    public string Content { get; init; } 
}