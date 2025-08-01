using System.Text.Json.Serialization;
using HealthBeside.Infrastructure;

namespace HealthBeside.Application.Contracts.Forum.ForumCommentDto;

public class GetForumCommentDto
{
    public Guid Id { get; init; }
    public string Content { get; init; }
    [JsonConverter(typeof(DateTimeLocalJsonConverter))]
    public DateTime CreatedAt { get; init; }
    [JsonConverter(typeof(DateTimeLocalJsonConverter))]
    public DateTime UpdatedAt { get; init; }
    public int Likes { get; init; } 
    public int Dislikes { get; init; } 
    public bool IsAnswer { get; init; } 
}