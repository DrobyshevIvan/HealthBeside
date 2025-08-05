using HealthBeside.Application.Contracts.Forum.ForumPostDto;

namespace HealthBeside.Application.Interfaces;

public interface IForumPostService
{
    Task<IEnumerable<GetForumPostDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GetDetailedForumPostDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetDetailedForumPostDto> CreateAsync(CreateForumPostDto forumPostDto, Guid authorId, CancellationToken cancellationToken = default);
    Task<GetUpdatedForumPostDto> UpdateAsync(UpdateForumPostDto updateForumPostDto, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
    
    // TODO Add methods for pagination, filtering, and sorting 
}