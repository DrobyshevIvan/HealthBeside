using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Application.Extensions.Mapping.Forum.ForumPostDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Forum;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

//TODO ADD CANCELLATION TOKENS TO ALL ASYNC METHODS, Валідація в DTO / FluentValidation
public class ForumPostService : IForumPostService
{
    private readonly IForumPostRepository _forumPostRepository;
    private readonly ILogger<ForumPostService> _logger;

    public ForumPostService(IForumPostRepository forumPostRepository, ILogger<ForumPostService> logger)
    {
        _forumPostRepository = forumPostRepository;
        _logger = logger;
    }

    private static void EnsureOwnership(ForumPost post, Guid userId)
    {
        if (post.AuthorId != userId)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }

    public async Task<IEnumerable<GetForumPostDto>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all forum posts.");
        var posts = await _forumPostRepository.GetAllAsync();
        return posts.Select(post => post.ToGetForumPostDto());
    }

    public async Task<GetDetailedForumPostDto> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting forum post by ID: {PostId}", id);
        var post = await _forumPostRepository.GetByIdWithAuthorAsync(id);

        if (post is null)
        {
            _logger.LogWarning("Forum post with ID {PostId} not found.", id);
            throw new ForumPostFindingException("Unknown error: ForumPost with Author ID not found.");
        }

        return post.ToGetDetailedForumPostDto();
    }

    public async Task<GetDetailedForumPostDto> CreateAsync(CreateForumPostDto forumPostDto, Guid authorId)
    {
        _logger.LogInformation("Creating forum post by user {UserId}", authorId);
        (string? error, ForumPost? forumPost) = ForumPost.Create(authorId, forumPostDto.Title, forumPostDto.Content);

        if (error != null)
        {
            _logger.LogWarning("Validation failed while creating forum post: {Error}", error);
            throw new ForumPostCreationException(error);
        }

        if (forumPost is null)
        {
            _logger.LogError("Forum post creation returned null.");
            throw new ForumPostCreationException("Unknown error: ForumPost is null.");
        }

        var addedPost = await _forumPostRepository.AddAsync(forumPost);
        var postWithAuthor = await _forumPostRepository.GetByIdWithAuthorAsync(addedPost.Id);

        if (postWithAuthor is null)
        {
            _logger.LogError("Forum post created but failed to retrieve with author info. PostId: {PostId}", addedPost.Id);
            throw new ForumPostCreationException("Unknown error: ForumPost with Author ID not found.");
        }

        _logger.LogInformation("Forum post created successfully. PostId: {PostId}", addedPost.Id);
        return postWithAuthor.ToGetDetailedForumPostDto();
    }

    public async Task<GetUpdatedForumPostDto> UpdateAsync(UpdateForumPostDto dto, Guid userId)
    {
        _logger.LogInformation("User {UserId} is updating forum post {PostId}", userId, dto.PostId);

        var post = await _forumPostRepository.GetAsync(dto.PostId);

        if (post is null)
        {
            _logger.LogWarning("Attempt to update non-existent forum post: {PostId}", dto.PostId);
            throw new KeyNotFoundException("Forum post not found.");
        }

        EnsureOwnership(post, userId);

        var error = post.Update(dto.Title, dto.Content);
        if (error is not null)
        {
            _logger.LogWarning("Validation failed while updating forum post {PostId}: {Error}", dto.PostId, error);
            throw new ArgumentException(error);
        }

        post.Touch();
        await _forumPostRepository.UpdateAsync(post);

        _logger.LogInformation("Forum post {PostId} updated successfully.", dto.PostId);
        return post.ToGetUpdatedForumPostDto();
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        _logger.LogInformation("User {UserId} is attempting to delete forum post {PostId}", userId, id);
        var post = await _forumPostRepository.GetAsync(id);

        if (post is null)
        {
            _logger.LogWarning("Attempt to delete non-existent forum post: {PostId}", id);
            return false;
        }

        EnsureOwnership(post, userId);
        await _forumPostRepository.DeleteAsync(id);

        _logger.LogInformation("Forum post {PostId} deleted successfully.", id);
        return true;
    }

    public async Task<bool> Exists(Guid id)
    {
        var post = await _forumPostRepository.GetAsync(id);
        bool exists = post is not null;
        _logger.LogDebug("Forum post {PostId} exists: {Exists}", id, exists);
        return exists;
    }
}
