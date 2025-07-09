using HealthBeside.Domain.Models.Users;

namespace HealthBeside.Domain.Models.Forum;

public class ForumComment
{
    public Guid Id { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int Likes { get; private set; } = 0;
    public int Dislikes { get; private set; } = 0;
    public bool IsAnswer { get; private set; } = false;
    
    public Guid PostId { get; private set; }
    public ForumPost Post { get; private set; }
    public string AuthorId { get; private set; }
    public ApplicationUser Author { get; private set; }
    
    public ICollection<ForumComment> Replies { get; private set; }
    public Guid? ParentCommentId { get; private set; } 
    public ForumComment ParentComment { get; private set; }
    
    // private readonly List<ForumComment> _replies = new List<ForumComment>();
    // private IReadOnlyCollection<ForumComment> _repliesReadOnly => _replies.AsReadOnly();
    
    private ForumComment() { }

    public static (string? Error, ForumComment? ForumComment) Create(string authorId, string content, Guid postId,
        Guid? parentCommentId)
    {
        var errors = new List<string>();
        
        if(postId == Guid.Empty)
            errors.Add("Post ID cannot be empty.");
        
        if(string.IsNullOrWhiteSpace(authorId))
            errors.Add("Author ID cannot be null.");
        
        if(string.IsNullOrWhiteSpace(content))
          errors.Add("Content cannot be empty.");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);

        var forumComment = new ForumComment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            AuthorId = authorId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            Likes = 0,
            Dislikes = 0,
            IsAnswer = false // За замовчуванням не є відповіддю
        };

        return (null, forumComment);
    }
    
    public static (string? Error, ForumComment? ForumComment) CreateReply(string authorId, string content, Guid postId,
        Guid parentCommentId)
    {
        var errors = new List<string>();
        
        if(postId == Guid.Empty)
            errors.Add("Post ID cannot be empty.");
        
        if(string.IsNullOrWhiteSpace(authorId))
            errors.Add("Author ID cannot be null.");
        
        if(string.IsNullOrWhiteSpace(content))
            errors.Add("Content cannot be empty.");
        
        if(parentCommentId == Guid.Empty)
            errors.Add("Parent comment ID cannot be empty.");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);

        var forumComment = new ForumComment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            AuthorId = authorId,
            ParentCommentId = parentCommentId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            Likes = 0,
            Dislikes = 0,
            IsAnswer = true 
        };

        return (null, forumComment);
        
     
    }
}