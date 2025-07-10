using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Forum;

namespace HealthBeside.Infrastructure.Repository;

public class ForumPostRepository : GenericRepository<ForumPost>, IForumPostRepository
{
    private readonly AppDbContext _context;

    public ForumPostRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}