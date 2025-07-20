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

    public async Task<IEnumerable<GetForumPostDto>> GetAll()
    {
        var posts = await _forumPostRepository.GetAllAsync();
        return posts.Select(post => post.ToGetForumPostDto());
    }

    public async Task<GetDetailedForumPostDto> GetById(Guid id)
    {
        var post = await _forumPostRepository.GetByIdWithAuthorAsync(id);
        
        if (post is null)
            throw new ForumPostCreationException("Unknown error: ForumPost with Author ID not found.");
        
        return post.ToGetDetailedForumPostDto();
    }

    public async Task<GetDetailedForumPostDto> Create(CreateForumPostDto forumPostDto, Guid authorId)
    {
        (string? error, ForumPost? forumPost) = forumPostDto.ToForumPost(authorId);

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

    public async Task<bool> Update(Guid id, UpdateForumPostDto updateForumPostDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Delete(Guid id)
    {
        var post = await _forumPostRepository.GetAsync(id);
        if (post is null)
            return false;

        await _forumPostRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> Exists(Guid id)
    {
        throw new NotImplementedException();
    }
}