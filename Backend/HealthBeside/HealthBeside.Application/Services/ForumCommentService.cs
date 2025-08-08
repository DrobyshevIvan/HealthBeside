using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.Forum.ForumCommentDto;
using HealthBeside.Application.Extensions.Mapping.Forum.ForumCommentDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Forum;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class ForumCommentService : IForumCommentService
{
    private readonly IForumCommentRepository _forumCommentRepository;
    private readonly ILogger<ForumCommentService> _logger;

    public ForumCommentService(IForumCommentRepository forumCommentRepository, ILogger<ForumCommentService> logger)
    {
        _forumCommentRepository = forumCommentRepository;
        _logger = logger;
    }

    private static void EnsureOwnership(ForumComment post, Guid userId)
    {
        if (post.AuthorId != userId)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }

    public async Task<IEnumerable<GetForumCommentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve all forum comments.");
        var comments = await _forumCommentRepository.GetAllAsync(cancellationToken);
        _logger.LogInformation("Successfully retrieved {Count} forum comments.", comments.Count());
        
        return comments.Select(comments => comments.ToGetForumCommentDto());
    }

    public async Task<GetForumCommentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve forum comment with ID: {CommentId}.", id);
        
        var comment = await _forumCommentRepository.GetAsync(id, cancellationToken);
    
        if (comment is null)
        {
            _logger.LogWarning("Comment with ID {CommentId} not found.", id);
            throw new KeyNotFoundException($"Comment with ID {id} not found.");
        }

        _logger.LogInformation("Successfully retrieved comment with ID: {CommentId}.", id);
        
        return comment.ToGetForumCommentDto();
    }

    public async Task<GetDetailedForumCommentDto> CreateAsync(
        CreateForumCommentDto forumCommentDto, 
        Guid authorId, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create a new forum comment for PostId: {PostId} by AuthorId: {AuthorId}.", 
            forumCommentDto.PostId, authorId);
        
        (string? error, ForumComment? forumComment) = ForumComment.Create(
            authorId,
            forumCommentDto.Content,
            forumCommentDto.PostId);

        if (error != null)
        {
            _logger.LogWarning("Failed to create ForumComment object due to validation error: {Error}", error);
            throw new ForumCommentCreationException(error);
        }

        if (forumComment is null)
        {
            _logger.LogError("ForumComment.Create returned a null object without an error message. This indicates an unexpected state.");
            throw new ForumCommentCreationException("Unknown error: ForumComment is null.");
        }

        var addedComment = await _forumCommentRepository.AddAsync(forumComment, cancellationToken);

        if (addedComment is null)
        {
            _logger.LogError("Repository.AddAsync returned a null object after successful creation. This indicates an unexpected state.");
            throw new InvalidOperationException("Unknown error: ForumComment could not be added.");
        }

        _logger.LogInformation("Successfully created a new comment with ID: {CommentId} for PostId: {PostId}.", 
            addedComment.Id, addedComment.PostId);
        
        return addedComment.ToGetDetailedForumCommentDto();
    }

    public async Task<GetUpdatedForumCommentDto> UpdateAsync(
        UpdateForumCommentDto updateForumCommentDto,
        Guid userId,
        CancellationToken cancellationToken = default) 
    {
        _logger.LogInformation("Attempting to update comment with ID: {CommentId} by user {UserId}.", 
            updateForumCommentDto.CommentId, userId);
        
        var comment = await _forumCommentRepository.GetAsync(updateForumCommentDto.CommentId, cancellationToken);
        
        if(comment is null)
        {
            _logger.LogWarning("Comment with ID {CommentId} not found for update.", updateForumCommentDto.CommentId);
            throw new KeyNotFoundException($"Comment with ID {updateForumCommentDto.CommentId} not found.");
        }
        
        EnsureOwnership(comment, userId);
        
        var error = comment.Update(
            updateForumCommentDto.Content,
            updateForumCommentDto.IsAnswer);
        
        if (error != null)
        {
            _logger.LogWarning("Validation failed for comment update {CommentId}: {Error}", updateForumCommentDto.CommentId, error);
            throw new ArgumentException(error);
        }
        
        comment.Touch();
        
        await _forumCommentRepository.UpdateAsync(comment, cancellationToken);
        
        _logger.LogInformation("Successfully updated comment with ID: {CommentId}.", comment.Id);
        
        return comment.ToGetUpdatedForumCommentDto();
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {UserId} is attempting to delete forum comment {CommentId}", userId, id);
        var comment = await _forumCommentRepository.GetAsync(id, cancellationToken);

        if (comment is null)
        {
            _logger.LogWarning("Attempt to delete non-existent forum comment: {CommentId}", id);
            return false;
        }

        EnsureOwnership(comment, userId);
        await _forumCommentRepository.DeleteAsync(id, cancellationToken);

        _logger.LogInformation("Forum comment {CommentId} deleted successfully by user {UserId}", id, userId);
        return true;
    }

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if comment with ID {CommentId} exists.", id);
        var comment = await _forumCommentRepository.GetAsync(id, cancellationToken);
        
        bool exists = comment is not null;
        _logger.LogDebug("Forum comment {CommentId} exists: {Exists}", id, exists);
        return exists;
    }
}