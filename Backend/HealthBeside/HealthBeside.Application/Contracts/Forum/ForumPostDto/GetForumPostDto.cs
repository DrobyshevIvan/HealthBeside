namespace HealthBeside.Application.Contracts.Forum.ForumPostDto;

public class GetForumPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public string Content { get; set; } 
    public DateTime CreatedAt { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
}