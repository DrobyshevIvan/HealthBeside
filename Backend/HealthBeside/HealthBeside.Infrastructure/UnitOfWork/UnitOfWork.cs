using HealthBeside.Domain.Interfaces;

namespace HealthBeside.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IForumCommentRepository ForumCommentRepository { get; }
    public IForumPostRepository ForumPostRepository { get; }

    public UnitOfWork(AppDbContext context,
        IForumCommentRepository forumCommentRepository,
        IForumPostRepository forumPostRepository)
    {
        _context = context;
        ForumCommentRepository = forumCommentRepository;
        ForumPostRepository = forumPostRepository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}