namespace HealthBeside.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IForumCommentRepository ForumCommentRepository { get; }
    public IForumPostRepository ForumPostRepository { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}