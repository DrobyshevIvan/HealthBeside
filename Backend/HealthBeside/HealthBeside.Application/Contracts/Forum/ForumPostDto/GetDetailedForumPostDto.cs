using System.Text.Json.Serialization;
using HealthBeside.Infrastructure;

namespace HealthBeside.Application.Contracts.Forum.ForumPostDto;

public class GetDetailedForumPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public string Content { get; set; } 
    
    [JsonConverter(typeof(DateTimeLocalJsonConverter))]
    public DateTime CreatedAt { get; set; }
    
    [JsonConverter(typeof(DateTimeLocalJsonConverter))]
    public DateTime UpdatedAt { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public GetUserDto Author { get; set; }
}