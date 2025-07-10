using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Forum;

namespace HealthBeside.Infrastructure.Repository;

public class ForumCommentRepository : GenericRepository<ForumComment>, IForumCommentRepository
{
    private readonly AppDbContext _context;

    public ForumCommentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}