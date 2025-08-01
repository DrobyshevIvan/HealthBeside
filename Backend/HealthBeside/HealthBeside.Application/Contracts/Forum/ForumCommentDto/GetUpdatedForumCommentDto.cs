using System.Text.Json.Serialization;
using HealthBeside.Infrastructure;

namespace HealthBeside.Application.Contracts.Forum.ForumCommentDto;

public class GetUpdatedForumCommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } 
    [JsonConverter(typeof(DateTimeLocalJsonConverter))]
    public DateTime UpdatedAt { get; set; }
    public bool IsAnswer { get; set; } = false;
    public int Likes { get; set; }
    public int Dislikes { get; set; }
}