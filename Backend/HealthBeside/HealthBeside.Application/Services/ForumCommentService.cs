using HealthBeside.Application.Contracts.Forum.ForumCommentDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Forum;

namespace HealthBeside.Application.Services;

//TODO implement this service and add custom exceptions for better error handling
public class ForumCommentService : IForumCommentService
{
    private readonly IForumCommentRepository _forumCommentRepository;

    public ForumCommentService(IForumCommentRepository forumCommentRepository)
    {
        _forumCommentRepository = forumCommentRepository;
    }


    public async Task<IEnumerable<GetForumCommentDto>> GetAllAsync()
    {
        var comments = await _forumCommentRepository.GetAllAsync();
        return comments.Select(comment => new GetForumCommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            Likes = comment.Likes,
            Dislikes = comment.Dislikes,
            IsAnswer = comment.IsAnswer
        });
    }

    public async Task<GetForumCommentDto> GetByIdAsync(Guid id)
    {
        var comment = await _forumCommentRepository.GetAsync(id);
    
        if (comment is null)
            throw new KeyNotFoundException($"Comment with ID {id} not found.");

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

    public async Task<GetDetailedForumCommentDto> CreateAsync(CreateForumCommentDto forumCommentDto, Guid authorId)
    {
        (string? error, ForumComment? forumComment) = ForumComment.Create(
            authorId,
            forumCommentDto.Content,
            forumCommentDto.PostId);

        if (error != null)
            throw new ArgumentException(error, nameof(forumCommentDto));

        if (forumComment is null)
            throw new InvalidOperationException("Unknown error: ForumComment is null.");

        var addedComment = await _forumCommentRepository.AddAsync(forumComment);

        if (addedComment is null)
            throw new InvalidOperationException("Unknown error: ForumComment could not be added.");

        return new GetDetailedForumCommentDto
        {
            Id = addedComment.Id,
            Content = addedComment.Content,
            CreatedAt = addedComment.CreatedAt,
            Likes = addedComment.Likes,
            Dislikes = addedComment.Dislikes,
            IsAnswer = addedComment.IsAnswer,
            AuthorId = addedComment.AuthorId,
            PostId = addedComment.PostId
        };
    }

    public async Task<bool> UpdateAsync(UpdateForumCommentDto updateForumCommentDto) 
    {
        var comment = await _forumCommentRepository.GetAsync(updateForumCommentDto.CommentId);
        
        if(comment is null)
            throw new KeyNotFoundException($"Comment with ID {updateForumCommentDto.CommentId} not found.");
        
        var error = comment.Update(
            updateForumCommentDto.Content,
            updateForumCommentDto.IsAnswer);
        if (error != null)
            throw new ArgumentException(error);
        
        await _forumCommentRepository.UpdateAsync(comment);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var comment = await _forumCommentRepository.GetAsync(id);

        if (comment is null)
            return false;
        
        await _forumCommentRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> Exists(Guid id)
    {
        var comment = await _forumCommentRepository.GetAsync(id);

        if (comment is null)
            return false;

        return true;
    }
}