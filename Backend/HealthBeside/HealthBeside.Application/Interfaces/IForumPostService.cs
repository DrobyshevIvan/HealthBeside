using HealthBeside.Application.Contracts.Forum.ForumPostDto;

namespace HealthBeside.Application.Interfaces;

public interface IForumPostService
{
    Task<IEnumerable<GetForumPostDto>> GetAll();
    Task<GetDetailedForumPostDto> GetById(Guid id);
    Task<GetDetailedForumPostDto> Create(CreateForumPostDto forumPostDto, Guid authorId);
    Task<bool> Update(Guid id, UpdateForumPostDto updateForumPostDto);
    Task<bool> Delete(Guid id);
    Task<bool> Exists(Guid id);
}