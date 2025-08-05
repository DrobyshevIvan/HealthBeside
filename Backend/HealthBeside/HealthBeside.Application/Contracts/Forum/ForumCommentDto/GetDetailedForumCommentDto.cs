using System.Text.Json.Serialization;
using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Infrastructure;

namespace HealthBeside.Application.Contracts.Forum.ForumCommentDto;

public class GetDetailedForumCommentDto
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
    public GetUserDto Author { get; init; }
    public Guid PostId { get; init; }
}