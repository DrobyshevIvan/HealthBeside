using HealthBeside.Application.Contracts.Forum.ForumCommentDto;

namespace HealthBeside.Application.Interfaces;

public interface IForumCommentService
{
    Task<IEnumerable<GetForumCommentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GetForumCommentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetDetailedForumCommentDto> CreateAsync(CreateForumCommentDto forumCommentDto, Guid authorId, CancellationToken cancellationToken = default);
    Task<GetUpdatedForumCommentDto> UpdateAsync(UpdateForumCommentDto updateForumCommentDto, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
    
    // TODO Add methods for pagination, filtering, and sorting 
}