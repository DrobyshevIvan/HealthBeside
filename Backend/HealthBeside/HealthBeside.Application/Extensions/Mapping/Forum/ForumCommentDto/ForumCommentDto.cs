using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.Forum.ForumCommentDto;
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
    
    public static GetUpdatedForumCommentDto ToGetUpdatedForumCommentDto(this ForumComment comment)
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
        return new GetDetailedForumCommentDto()
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            Likes = comment.Likes,
            Dislikes = comment.Dislikes,
            IsAnswer = comment.IsAnswer,
            Author = comment.Author == null
                ? null
                : new GetUserDto 
                { 
                    FirstName = comment.Author.FirstName, 
                    LastName = comment.Author.LastName 
                },
            PostId = comment.PostId
        };
    }
}