using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Domain.Models.Forum;

namespace HealthBeside.Application.Extensions.Mapping.Forum.ForumPostDto;

public static class CreateForumPostDtoExtension
{
    public static (string? error, ForumPost? forumPost) ToForumPost(this CreateForumPostDto forumPostDto, Guid authorId)
    {
        return ForumPost.Create(
            authorId,
            forumPostDto.Title,
            forumPostDto.Content);
    }
}