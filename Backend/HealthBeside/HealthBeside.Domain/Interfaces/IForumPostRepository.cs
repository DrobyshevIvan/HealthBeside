using HealthBeside.Domain.Models.Forum;
using HealthBeside.Domain.Models.Shared;

namespace HealthBeside.Domain.Interfaces;

public interface IForumPostRepository : IGenericRepository<ForumPost>
{
    Task<ForumPost?> GetByIdWithAuthorAsync(Guid id);
}