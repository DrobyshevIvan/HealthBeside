using HealthBeside.Application.Contracts.Forum.ForumCommentDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Interfaces;

namespace HealthBeside.Application.Services;

//TODO implement this service
public class ForumCommentService : IForumCommentService
{
    private readonly IForumCommentRepository _forumCommentRepository;

    public ForumCommentService(IForumCommentRepository forumCommentRepository)
    {
        _forumCommentRepository = forumCommentRepository;
    }


    public Task<IEnumerable<GetForumCommentDto>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<GetDetailedForumCommentDto> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<CreateForumCommentDto> Create(CreateForumCommentDto forumCommentDto, Guid authorId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(Guid id, UpdateForumCommentDto updateForumCommentDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(Guid id)
    {
        throw new NotImplementedException();
    }
}