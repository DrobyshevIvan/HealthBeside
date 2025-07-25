using HealthBeside.Application.Contracts.Forum.ForumCommentDto;

namespace HealthBeside.Application.Interfaces;

public interface IForumCommentService
{
    Task<IEnumerable<GetForumCommentDto>> GetAll();
    Task<GetDetailedForumCommentDto> GetById(Guid id);
    Task<CreateForumCommentDto> Create(CreateForumCommentDto forumCommentDto, Guid authorId);
    Task<bool> Update(Guid id, UpdateForumCommentDto updateForumCommentDto);
    Task<bool> Delete(Guid id);
    Task<bool> Exists(Guid id);
}