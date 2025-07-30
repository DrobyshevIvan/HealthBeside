using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Application.Extensions.Mapping.Forum.ForumPostDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Forum;

namespace HealthBeside.Application.Services;

public class ForumPostService : IForumPostService
{
    private readonly IForumPostRepository _forumPostRepository;

    public ForumPostService(IForumPostRepository forumPostRepository)
    {
        _forumPostRepository = forumPostRepository;
    }

    public async Task<IEnumerable<GetForumPostDto>> GetAllAsync()
    {
        var posts = await _forumPostRepository.GetAllAsync();
        return posts.Select(post => post.ToGetForumPostDto());
    }

    public async Task<GetDetailedForumPostDto> GetByIdAsync(Guid id)
    {
        var post = await _forumPostRepository.GetByIdWithAuthorAsync(id);
        
        if (post is null)
            throw new ForumPostFindingException("Unknown error: ForumPost with Author ID not found.");
        
        return post.ToGetDetailedForumPostDto();
    }

    public async Task<GetDetailedForumPostDto> CreateAsync(CreateForumPostDto forumPostDto, Guid authorId)
    {
        (string? error, ForumPost? forumPost) = ForumPost.Create(authorId, forumPostDto.Title, forumPostDto.Content);

        if (error != null)
        {
            throw new ForumPostCreationException(error);
        }
        
        if (forumPost is null)
        {
            throw new ForumPostCreationException("Unknown error: ForumPost is null.");
        }
        
        var addedPost = await _forumPostRepository.AddAsync(forumPost);

        var postWithAuthor = await _forumPostRepository.GetByIdWithAuthorAsync(addedPost.Id);
        
        if (postWithAuthor is null)
            throw new ForumPostCreationException("Unknown error: ForumPost with Author ID not found.");
        
        return postWithAuthor.ToGetDetailedForumPostDto();
    }

    public async Task<GetUpdatedForumPostDto> UpdateAsync(UpdateForumPostDto dto, Guid userId)
    {
        var post = await _forumPostRepository.GetAsync(dto.PostId);

        if (post is null)
            throw new KeyNotFoundException("Forum post not found.");
        
        if(post.AuthorId != userId)
            throw new UnauthorizedAccessException("You are not authorized to update this post.");

        var error = post.Update(dto.Title, dto.Content);

        if (error is not null)
            throw new ArgumentException(error);
        
        post.Touch();
        
        await _forumPostRepository.UpdateAsync(post);

        return post.ToGetUpdatedForumPostDto();
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var post = await _forumPostRepository.GetAsync(id);
        if (post is null)
            return false;
        
        if(post.AuthorId != userId)
            throw new UnauthorizedAccessException("You are not authorized to delete this post.");

        await _forumPostRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> Exists(Guid id)
    {
        var post = await _forumPostRepository.GetAsync(id);
        
        if(post is null)
           return false;
        
        return true;
    }
}