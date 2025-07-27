using HealthBeside.Application.Contracts.Forum.ForumCommentDto;

namespace HealthBeside.Application.Interfaces;

public interface IForumCommentService
{
    Task<IEnumerable<GetForumCommentDto>> GetAllAsync();
    Task<GetForumCommentDto> GetByIdAsync(Guid id);
    Task<GetDetailedForumCommentDto> CreateAsync(CreateForumCommentDto forumCommentDto, Guid authorId);
    Task<bool> UpdateAsync(UpdateForumCommentDto updateForumCommentDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> Exists(Guid id);
}