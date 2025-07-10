using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Forum;

public class ForumPost
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } 
    public string Content { get; private set; } 
    public DateTime CreatedAt { get; private set; }
    public int Likes { get; private set; } = 0;
    public int Dislikes { get; private set; } = 0;
    
    public string AuthorId { get; private set; }
    public ApplicationUser Author { get; private set; }
    
    public ICollection<ForumComment> Comments { get; private set; }
    // private readonly List<ForumComment> _comments = new List<ForumComment>();
    // public IReadOnlyCollection<ForumComment> Comments => _comments.AsReadOnly();
    
    private ForumPost() {}
    
    public static (string? Error, ForumPost? ForumPost) Create(string authorId, string title, string content)
    {
        var errors = new List<string>();
        
        if(authorId == null) 
            errors.Add("Author ID cannot be null.");
        
        if(string.IsNullOrWhiteSpace(title))
            errors.Add("Title cannot be empty.");
        
        if(string.IsNullOrWhiteSpace(content))
            errors.Add("Content cannot be empty.");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);
        
        var forumPost = new ForumPost
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            Title = title,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            Likes = 0,
            Dislikes = 0
        };
        
        return (null, forumPost);
    }
}