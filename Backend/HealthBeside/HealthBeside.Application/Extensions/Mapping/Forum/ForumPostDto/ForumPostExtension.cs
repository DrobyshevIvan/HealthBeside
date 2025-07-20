using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Application.Extensions.Mapping.User;
using HealthBeside.Domain.Models.Forum;

namespace HealthBeside.Application.Extensions.Mapping.Forum.ForumPostDto;

public static class ForumPostExtension
{
    public static GetForumPostDto ToGetForumPostDto(this ForumPost post)
    {
        return new GetForumPostDto
        {
            Id = post.Id,
            Content = post.Content,
            Title = post.Title,
            CreatedAt = post.CreatedAt,
            Dislikes = post.Dislikes,
            Likes = post.Likes
        };
    }

    public static GetDetailedForumPostDto ToGetDetailedForumPostDto(this ForumPost post)
    {
        return new GetDetailedForumPostDto
        {
            Id = post.Id,
            Content = post.Content,
            Title = post.Title,
            CreatedAt = post.CreatedAt,
            Dislikes = post.Dislikes,
            Likes = post.Likes,
            Author = post.Author.ToGetUserDto()
        };
    }
}