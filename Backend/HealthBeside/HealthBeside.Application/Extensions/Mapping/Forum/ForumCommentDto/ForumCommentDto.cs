using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.Forum.ForumCommentDto;
using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Application.Extensions.Mapping.User;
using HealthBeside.Domain.Models.Forum;

namespace HealthBeside.Application.Extensions.Mapping.Forum.ForumCommentDto;

public static class ForumCommentDto
{
    public static GetForumCommentDto ToGetForumCommentDto(this ForumComment comment)
    {
        return new GetForumCommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            Likes = comment.Likes,
            Dislikes = comment.Dislikes,
            IsAnswer = comment.IsAnswer
        };
    }
    
    public static GetUpdatedForumCommentDto ToGetUpdatedForumPostDto(this ForumComment comment)
    {
        return new GetUpdatedForumCommentDto()
        {
            Id = comment.Id,
            Content = comment.Content,
            IsAnswer = comment.IsAnswer,
            UpdatedAt = comment.UpdatedAt,
            Likes = comment.Likes,
            Dislikes = comment.Dislikes
        };
    }

    public static GetDetailedForumCommentDto ToGetDetailedForumCommentDto(this ForumComment comment)
    {
        return new GetDetailedForumPostDto
        {
            Id = comment.Id,
            Content = comment.Content,
            Title = comment.Title,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Dislikes = comment.Dislikes,
            Likes = comment.Likes,
            Author = comment.Author.ToGetUserDto()
        };
    }
}