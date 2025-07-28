using HealthBeside.Application.Contracts.Forum.ForumPostDto;

namespace HealthBeside.Application.Interfaces;

public interface IForumPostService
{
    Task<IEnumerable<GetForumPostDto>> GetAllAsync();
    Task<GetDetailedForumPostDto> GetByIdAsync(Guid id);
    Task<GetDetailedForumPostDto> CreateAsync(CreateForumPostDto forumPostDto, Guid authorId);
    Task<bool> UpdateAsync(UpdateForumPostDto updateForumPostDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> Exists(Guid id);
}