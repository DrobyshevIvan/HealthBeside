using System.Runtime.CompilerServices;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Forum;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repositories;

public class ForumPostRepository : GenericRepository<ForumPost>, IForumPostRepository
{
    private readonly AppDbContext _context;

    public ForumPostRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ForumPost?> GetByIdWithAuthorAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ForumPosts
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
    
}